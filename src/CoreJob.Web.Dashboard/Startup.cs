using CoreJob.Framework;
using CoreJob.Framework.Json.Extensions;
using CoreJob.Framework.Models;
using CoreJob.Server.Framework;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Store.Mysql;
using CoreJob.Web.Dashboard.Helpers.Common;
using CoreJob.Web.Dashboard.Hubs;
using CoreJob.Web.Dashboard.Services.JobAction;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CoreJob.Web.Dashboard
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var ass = new[] { typeof(Startup).Assembly };
            services.AddMediatR(ass);
            services.AddValidatorsFromAssemblies(ass, ServiceLifetime.Scoped);
            services.Add(new ServiceDescriptor(typeof(IValidationNotificationContext), typeof(ValidationNotificationContext), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>), ServiceLifetime.Scoped));

            services.AddControllersWithViews().AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.WriteIndented = true;
                option.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicySpan();
                option.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
            });

            services.AddAuthentication("CoreJobCookie").AddCookie("CoreJobCookie", p =>
            {
                p.Cookie.HttpOnly = true;
                p.ExpireTimeSpan = TimeSpan.FromDays(7);
                p.LoginPath = new PathString("/account/login");
                p.LogoutPath = new PathString("/account/logout");
                p.SlidingExpiration = true;
                p.ReturnUrlParameter = "redirect";
            });

            services.AddCoreJobServer(_configuration, options => 
            {
                options.InputMessageType = HttpMessageType.MSGPACK;
                options.OutputMessageType = HttpMessageType.MSGPACK;
                options.Token = "LDgVTSL2m3oEZMvgMAtJzEhhD8rT0bRpQXQ8583E";
                options.JobAssemblies.Add(typeof(Startup).Assembly);
                options.UseMysql(x =>
                {
                    x.StoreConnectionStr = _configuration.GetEnvironmentOrConfigStr("ConnectionStrings:CoreJobConnection");
                });
            });

            services.AddSignalR();
            services.AddSingleton<ISystemScheduler, CupSystemScheduler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCoreJobServer();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<CpuHub>("/hub/cpu");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
