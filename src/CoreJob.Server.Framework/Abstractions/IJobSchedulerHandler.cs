using System.Threading.Tasks;
using CoreJob.Framework.Models;
using Quartz;

namespace CoreJob.Server.Framework.Abstractions
{
    public interface IJobSchedulerHandler
    {
        Task AddOrUpdateJob(JobInfo job, bool trigger);

        Task TriggerJob(int jobId, JobDataMap jobData);

        Task<bool> DeleteJob(int jobId);

        Task Resume(JobInfo job);

        Task Pause(JobInfo job);

        Task SystemScheduler();

        Task StopAll();
    }
}
