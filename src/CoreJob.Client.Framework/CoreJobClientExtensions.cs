using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using CoreJob.Client.Framework.Abstractions;
using CoreJob.Client.Framework.Logger;
using CoreJob.Client.Framework.Models;
using CoreJob.Client.Framework.Services;
using CoreJob.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace CoreJob.Client.Framework
{
    public static class CoreJobClientExtensions
    {
        public static IServiceCollection AddCoreJobClient(this IServiceCollection services, IConfiguration configuration, Action<CoreJobClientOptions> action = null)
        {
            // options
            var options = new CoreJobClientOptions();
            action?.Invoke(options);
            services.AddOptionConfiguration(configuration, options);
            services.AddSingleton(options);

            // http
            services.AddHttpContextAccessor();

            // services
            services.AddSingleton<ICoreJobExecutor, CoreJobExecutor>();
            services.AddSingleton<IProcessor, ExecutorRegistryProcessor>();
            services.AddTransient<IRestClient>(p =>
            {
                var conf = p.GetRequiredService<CoreJobClientOptions>();
                var client = new RestClient(conf.AdminUrl);
                client.RemoteCertificateValidationCallback = 
                    (sender, certificate, chain, sslPolicyErrors) => true;
                return client;
            });
            services.AddSingleton<IJobDispatcher, JobDispatcher>();
            services.AddSingleton<IJobSender, CoreJobSender>();
            services.AddTransient<CoreJobQueue>();
            services.AddSingleton<IJobLoggerStore, LiteDBLoggerStore>();

            // route table
            services.AddRouteTable(new ClientRouteTable());

            // host service
            services.AddHostedService<CoreJobBackgroundService>();

            // job handler
            services.AddSingleton(typeof(ICoreJobServicesProvider<>), typeof(CoreJobServicesProvider<>));
            services.ScanDynamicJobExecutorHandler(options.HandlerServiceLifetime);

            return services;
        }

        public static IApplicationBuilder UseCoreJobClient(this IApplicationBuilder app, Action<JobLoggerConfiguration> logAction = null)
        {
            app.UseMiddleware<CoreJobClientMiddleware>();

            var jobLoggerConfiguration = new JobLoggerConfiguration();
            logAction?.Invoke(jobLoggerConfiguration);
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddProvider(new CustomJobLoggerProvider(jobLoggerConfiguration, app.ApplicationServices.GetRequiredService<IJobLoggerStore>()));
            return app;
        }

        private static IEnumerable<Type> FilterByInterface(this IEnumerable<Type> types, Type interfaceType)
        {
            return types.Where(i => interfaceType.IsAssignableFrom(i) && i != interfaceType);
        }

        internal delegate Func<string, dynamic> ScopedDynamicDelegate(IServiceProvider serviceProvider);

        private static void AddScopedDynamicJobHandler(this IServiceCollection services, Type interfaceType, IEnumerable<Type> types, ServiceLifetime lifetime)
        {
            var typeFunc = typeof(Func<,>).MakeGenericType(typeof(string), interfaceType);
            ScopedDynamicDelegate f = serviceProvider => tenant => {
                var type = types.FilterByInterface(interfaceType)
                               .FirstOrDefault(x => x.GetCustomAttribute<CoreJobHandlerAttribute>()?.Name == tenant);

                if (null == type)
                    throw new KeyNotFoundException("No instance found for the given tenant.");

                return serviceProvider.GetService(type);
            };

            services.Add(new ServiceDescriptor(typeFunc, serviceProvider => f(serviceProvider), lifetime));
        }

        private static void ScanDynamicJobExecutorHandler(this IServiceCollection services, ServiceLifetime lifetime)
        {
            // get all types
            var typesToRegisterAndAttr = JobUtilExtensions.CollectByAttributeTypes<CoreJobHandlerAttribute>()
                    .Where(x => x.Item1.IsClass && !x.Item1.IsAbstract).ToList();

            var types = typesToRegisterAndAttr.Select(x =>
            {
                if (x.Item2.InterfaceType == null)
                {
                    // get default
                    var interfaces = x.Item1.GetInterfaces();

                    return (x.Item1, interfaces.FirstOrDefault());
                }

                return (x.Item1, x.Item2.InterfaceType);
            });

            var typesToRegisterList = types.GroupBy(x => x.Item2, y => y).ToDictionary(x => x.Key, y => y.ToList());

            typesToRegisterAndAttr.ForEach(x => services.Add(new ServiceDescriptor(x.Item1, x.Item1, lifetime)));

            if (typesToRegisterList != null)
            {
                foreach (var typesToRegister in typesToRegisterList)
                {
                    if (typesToRegister.Value != null)
                    {
                        services.AddScopedDynamicJobHandler(typesToRegister.Key, typesToRegister.Value.Select(x => x.Item1), lifetime);
                    }
                }
            }
        }
    }
}
