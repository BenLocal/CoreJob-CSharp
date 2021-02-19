using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Abstractions;
using Microsoft.Extensions.Logging;
using Quartz;
using CoreJob.Server.Framework;
using Microsoft.AspNetCore.SignalR;
using CoreJob.Web.Dashboard.Hubs;
using System.Diagnostics;

namespace CoreJob.Web.Dashboard.Services.JobAction
{
    public class CupSystemScheduler : ISystemScheduler
    {
        private readonly ILogger _logger;

        private readonly ISchedulerFactory _schedulerFactory;

        public CupSystemScheduler(ILogger<CupSystemScheduler> logger,
            ISchedulerFactory schedulerFactory)
        {
            _logger = logger;
            _schedulerFactory = schedulerFactory;
        }


        public async Task Start()
        {
            _logger.LogInformation("开始启动CpuJob");

            var jobKey = typeof(CupJob).GetSystemJobKey();
            var triggerKey = typeof(CupJob).GetSystemTriggerKey();

            var job = JobBuilder.Create<CupJob>()
                   .WithIdentity(jobKey)
                   .StoreDurably()
                   .Build();

            var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(jobKey)
                    .StartNow()
                    .WithCronSchedule("0/2 * * * * ? *")
                    .Build();

            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.ScheduleJob(job, trigger);

            _logger.LogInformation("成功启动CpuJob");
        }
    }

    public class CupJob : IJob
    {
        private readonly IHubContext<CpuHub> _hubContext;

        public CupJob(IHubContext<CpuHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var data = await GetCpuUsageForProcess();
            await _hubContext.Clients.All.SendAsync("PushCupData", DateTime.Now.ToString("HH:mm:ss"), data.ToString("G2"));
        }

        private async Task<double> GetCpuUsageForProcess()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            // Process.GetCurrentProcess().WorkingSet64 / 1024/ 1024;
            await Task.Delay(500);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return cpuUsageTotal * 100;
        }
    }
}
