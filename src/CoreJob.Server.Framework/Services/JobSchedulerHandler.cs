using System.Collections.Generic;
using System.Threading.Tasks;
using CoreJob.Framework;
using CoreJob.Framework.Models;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CoreJob.Server.Framework.Services
{
    public class JobSchedulerHandler : IJobSchedulerHandler
    {
        private readonly ISchedulerFactory _schedulerFactory;

        private readonly IEnumerable<ISystemScheduler> _systemSchedulers;

        public JobSchedulerHandler(ISchedulerFactory schedulerFactory,
            IEnumerable<ISystemScheduler> systemSchedulers)
        {
            _systemSchedulers = systemSchedulers;
            _schedulerFactory = schedulerFactory;
        }

        public async Task AddOrUpdateJob(JobInfo job, bool trigger)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            if (trigger)
            {
                await CreateTriggerAndStart(job);
            }
            else
            {
                await TryAddJob(scheduler, job.Id);
            }
        }

        public async Task<bool> DeleteJob(int jobId)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var triggerKey = jobId.GetTriggerKey();
            if (await scheduler.CheckExists(triggerKey) && !await scheduler.UnscheduleJob(triggerKey))
            {
                return false;
            }

            var jobKey = jobId.GetJobKey();
            if (await scheduler.CheckExists(jobKey))
            {
                await scheduler.DeleteJob(jobKey);
            }

            return true;
        }

        public async Task Pause(JobInfo job)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var triggerKey = job.GetTriggerKey();

            if (await scheduler.CheckExists(triggerKey))
            {
                await scheduler.PauseTrigger(triggerKey);
            }         
        }

        public async Task Resume(JobInfo job)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var triggerKey = job.GetTriggerKey();

            if (await scheduler.CheckExists(triggerKey))
            {
                await scheduler.ResumeTrigger(triggerKey);
            }
            else
            {
                await CreateTriggerAndStart(job);
            }
        }

        public async Task StopAll()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.PauseAll();
            await scheduler.Clear();
        }

        public async Task SystemScheduler()
        {
            if (_systemSchedulers != null)
            {
                foreach (var item in _systemSchedulers)
                {
                    await item.Start();
                }
            }
        }

        public async Task TriggerJob(int jobId, JobDataMap jobData)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await TryAddJob(scheduler, jobId);

            var jobKey = jobId.GetJobKey();
            await scheduler.TriggerJob(jobKey, jobData);
        }

        private async Task CreateTriggerAndStart(JobInfo job)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            await TryAddJob(scheduler, job.Id);

            var data = new JobDataMap();
            data.SetMapData(job);

            ITrigger newJobTrigger = TriggerBuilder.Create()
                       .WithIdentity(job.GetTriggerKey())
                       .ForJob(job.GetJobKey())
                       .UsingJobData(data)
                       .WithCronSchedule(job.Cron)
                       .Build();

            if (await scheduler.CheckExists(job.GetTriggerKey()))
            {
                await scheduler.RescheduleJob(job.GetTriggerKey(), newJobTrigger);
            }
            else
            {
                await scheduler.ScheduleJob(newJobTrigger);
            }
        }

        private async Task TryAddJob(IScheduler scheduler, int jobId)
        {
            // define the job and tie it to our HelloJob class
            IJobDetail jobDetail = JobBuilder.Create<DefaultCoreJob>()
                .WithIdentity(jobId.GetJobKey())
                .StoreDurably()
                .Build();

            if (!await scheduler.CheckExists(jobId.GetJobKey()))
            {
                await scheduler.AddJob(jobDetail, false);
            }
        }
    }
}
