using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreJob.Client.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Client.AspnetWeb
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCoreJobClient(_configuration, options =>
            {
                options.AdminUrl = "http://localhost:7800";
                options.HeartbeatIntervalSecond = 30;
                options.ExecutorUrl = "http://localhost:7801";
                options.InputMessageType = CoreJob.Framework.Models.HttpMessageType.MSGPACK;
                options.OutputMessageType = CoreJob.Framework.Models.HttpMessageType.MSGPACK;
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

            app.UseCoreJobClient();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
