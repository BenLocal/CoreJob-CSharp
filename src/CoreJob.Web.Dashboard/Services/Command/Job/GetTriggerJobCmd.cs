using System.Threading;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using MediatR;

namespace CoreJob.Web.Dashboard.Services.Command.Job
{
    public class GetTriggerJobCmd : IRequest<JobTriggerViewModel>
    {
        public int JobId { get; set; }

        public class JobTriggerCmdRequest : IRequestHandler<GetTriggerJobCmd, JobTriggerViewModel>
        {
            private readonly JobDbContext _dbContext;

            public JobTriggerCmdRequest(JobDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<JobTriggerViewModel> Handle(GetTriggerJobCmd request, CancellationToken cancellationToken)
            {
                var jobInfo = await _dbContext.JobInfo.FindAsync(request.JobId);

                return new JobTriggerViewModel()
                {
                    JobId = jobInfo.Id,
                    JobName = jobInfo.Name,
                    ExecutorParam = jobInfo.ExecutorParam
                };
            }
        }
    }
}
