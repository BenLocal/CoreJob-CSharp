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
    public class ClearLogsScheduler : ISystemScheduler
    {
        private readonly ILogger _logger;

        private readonly ISchedulerFactory _schedulerFactory;

        public ClearLogsScheduler(ILogger<ClearLogsScheduler> logger,
            ISchedulerFactory schedulerFactory)
        {
            _logger = logger;
            _schedulerFactory = schedulerFactory;
        }

        public async Task Start()
        {
            _logger.LogInformation("开始启动ClearLogsJob");

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

            _logger.LogInformation("成功启动ClearLogsJob");
        }
    }
}
