using System.Threading;
using System.Threading.Tasks;
using CoreJob.Framework.Models;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using MediatR;

namespace CoreJob.Web.Dashboard.Services.Command.Job
{
    public class DeleteJobCmd : IRequest<JsonEntityBase>
    {
        public int JobId { get; set; }

        public class DeleteJobCmdRequest : IRequestHandler<DeleteJobCmd, JsonEntityBase>
        {
            private readonly JobDbContext _dbContext;

            private readonly IJobSchedulerHandler _jobSchedulerHandler;

            public DeleteJobCmdRequest(JobDbContext dbContext,
                IJobSchedulerHandler jobSchedulerHandler)
            {
                _dbContext = dbContext;
                _jobSchedulerHandler = jobSchedulerHandler;
            }

            public async Task<JsonEntityBase> Handle(DeleteJobCmd request, CancellationToken cancellationToken)
            {
                _dbContext.JobInfo.Remove(new JobInfo() { Id = request.JobId });
                if (await _jobSchedulerHandler.DeleteJob(request.JobId))
                {
                    await _dbContext.SaveChangesAsync();
                }

                return new
                {
                }.Success();
            }
        }
    }
}
