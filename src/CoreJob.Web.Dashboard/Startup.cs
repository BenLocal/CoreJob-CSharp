using System;
using CoreJob.Framework.Models;
using CoreJob.Server.Framework;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Store.SqlServer;
using CoreJob.Web.Dashboard.Helpers.Common;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

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
            services.AddValidatorsFromAssemblies(new[] { typeof(Startup).Assembly },
                ServiceLifetime.Scoped);
            services.Add(new ServiceDescriptor(typeof(IValidationNotificationContext), typeof(ValidationNotificationContext), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>), ServiceLifetime.Scoped));

            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
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

            services.AddSingleton<IStoreProvider, MysqlServerStoreProvider>();
            services.AddCoreJobServer(_configuration, options => 
            {
                options.InputMessageType = HttpMessageType.MSGPACK;
                options.OutputMessageType = HttpMessageType.MSGPACK;
                options.Token = "LDgVTSL2m3oEZMvgMAtJzEhhD8rT0bRpQXQ8583E";
            });
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
