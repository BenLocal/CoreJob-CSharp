using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoreJob.Framework;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CoreJob.Server.Framework.Services
{
    public class RegistryHealthCheckScheduler : ISystemScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;

        private readonly ILogger _logger;

        public RegistryHealthCheckScheduler(ISchedulerFactory schedulerFactory,
            ILogger<RegistryHealthCheckScheduler> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        public async Task Start()
        {
            _logger.LogInformation("开始启动RegistryHealthCheckJob");

            var jobKey = typeof(RegistryHealthCheckJob).GetSystemJobKey();
            var triggerKey = typeof(RegistryHealthCheckJob).GetSystemTriggerKey();

            var job = JobBuilder.Create<RegistryHealthCheckJob>()
                   .WithIdentity(jobKey)
                   .StoreDurably()
                   .Build();

            var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(jobKey)
                    .StartNow()
                    .WithCronSchedule("30 * * * * ? *")
                    .Build();

            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.ScheduleJob(job, trigger);

            _logger.LogInformation("成功启动RegistryHealthCheckJob");
        }
    }
}
