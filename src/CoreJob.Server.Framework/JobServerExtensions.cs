using System;
using System.Net;
using CoreJob.Dashboard.Services;
using CoreJob.Framework;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Jobs;
using CoreJob.Server.Framework.Listener;
using CoreJob.Server.Framework.Models;
using CoreJob.Server.Framework.Services;
using CoreJob.Server.Framework.Store;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl.Matchers;
using Z.EntityFramework.Extensions;

namespace CoreJob.Server.Framework
{
    public static class JobServerExtensions
    {
        public static IServiceCollection AddCoreJobServer(this IServiceCollection services,
            IConfiguration configuration, Action<CoreJobServerOptions> action = null)
        {
            // options
            var options = new CoreJobServerOptions();
            action?.Invoke(options);
            services.AddOptionConfiguration(configuration, options);
            services.AddSingleton(options);

            // ef core
            EntityFrameworkManager.IsCommunity = true;
            services.AddDbContext<JobDbContext>((provider, options) =>
            {
                var storeOptions = provider.GetRequiredService<IOptions<StoreOptions>>().Value;
                provider.GetRequiredService<IStoreProvider>()?.OptionsAction(options, storeOptions);
                options.EnableSensitiveDataLogging().EnableDetailedErrors();
            });     

            // quartz
            services.AddQuartz(q =>
            {
                q.AddJobListener<CoreJobListener>(GroupMatcher<JobKey>.GroupEquals(JobConstant.Job_Default_Group));
                q.AddTriggerListener<CoreJobListener>(GroupMatcher<TriggerKey>.GroupEquals(JobConstant.Job_Default_Group));
                q.UseMicrosoftDependencyInjectionScopedJobFactory();
            });

            // services
            services.AddTransient<IServerSender, ServerSender>();
            services.AddSingleton<IJobSchedulerHandler, JobSchedulerHandler>();
            services.AddHostedService<JobSchedulerService>();
            services.AddSingleton<ISystemScheduler, RegistryHealthCheckScheduler>();

            // job
            if (!options.JobAssemblies.Contains(typeof(JobServerExtensions).Assembly))
            {
                options.JobAssemblies.Add(typeof(JobServerExtensions).Assembly);
            }

            JobUtilExtensions.CollectByInterface<IJob>(options.JobAssemblies).ForEach(t =>
            {
                services.AddScoped(t);
            });

            // route table
            services.AddRouteTable(new ServerRouteTable());

            // quartz server
            services.AddQuartzServer(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            foreach (var item in options.Extensions)
            {
                item.AddServices(services);
            }

            return services;
        }

        public static IApplicationBuilder UseCoreJobServer(this IApplicationBuilder app)
        {
            app.UseMiddleware<CoreJobServerMiddleware>();
            return app;
        }
    }
}
