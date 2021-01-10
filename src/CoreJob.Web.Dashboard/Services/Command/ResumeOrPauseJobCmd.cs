using System.Threading;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using MediatR;

namespace CoreJob.Web.Dashboard.Services.Command
{
    public class ResumeOrPauseJobCmd : IRequest<JsonEntityBase>
    {
        public int JobId { get; set; }

        public bool Resume { get; set; }

        public bool Pause { get; set; }

        public class JobTriggerCmdRequest : IRequestHandler<ResumeOrPauseJobCmd, JsonEntityBase>
        {
            private readonly JobDbContext _dbContext;

            private readonly IJobSchedulerHandler _jobSchedulerHandler;

            public JobTriggerCmdRequest(JobDbContext dbContext,
                IJobSchedulerHandler jobSchedulerHandler)
            {
                _dbContext = dbContext;
                _jobSchedulerHandler = jobSchedulerHandler;
            }

            public async Task<JsonEntityBase> Handle(ResumeOrPauseJobCmd request, CancellationToken cancellationToken)
            {
                var jobInfo = await _dbContext.JobInfo.FindAsync(request.JobId);
                if (request.Pause)
                {
                    jobInfo.Status = 0;
                    await _jobSchedulerHandler.Pause(jobInfo);
                }
                else if (request.Resume)
                {
                    jobInfo.Status = 1;
                    await _jobSchedulerHandler.Resume(jobInfo);
                }

                await _dbContext.SaveChangesAsync();

                return new { }.Success();
            }
        }
    }
}
