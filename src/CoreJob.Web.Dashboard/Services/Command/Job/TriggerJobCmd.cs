using System.Threading;
using System.Threading.Tasks;
using CoreJob.Server.Framework;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Helpers.Common;
using CoreJob.Web.Dashboard.Models;
using MediatR;
using Quartz;

namespace CoreJob.Web.Dashboard.Services.Command.Job
{
    public class TriggerJobCmd : IRequest<JsonEntityBase>
    {
        public JobTriggerViewModel VM { get; set; }

        public class PostJobTriggerCmdRequest : IRequestHandler<TriggerJobCmd, JsonEntityBase>
        {
            private readonly JobDbContext _dbContext;

            private readonly IJobSchedulerHandler _jobSchedulerHandler;

            private readonly IValidationNotificationContext _validationNotificationContext;

            public PostJobTriggerCmdRequest(JobDbContext dbContext,
                IJobSchedulerHandler jobSchedulerHandler,
                IValidationNotificationContext validationNotificationContext)
            {
                _dbContext = dbContext;
                _jobSchedulerHandler = jobSchedulerHandler;
                _validationNotificationContext = validationNotificationContext;
            }

            public async Task<JsonEntityBase> Handle(TriggerJobCmd request, CancellationToken cancellationToken)
            {
                if (_validationNotificationContext.HasErrorNotifications)
                {
                    return _validationNotificationContext.GetErrorNotifications().Error();
                }

                var jobInfo = await _dbContext.JobInfo.FindAsync(request.VM.JobId);

                jobInfo.ExecutorParam = request.VM.ExecutorParam;

                var data = new JobDataMap();
                data.SetMapData(jobInfo);
                await _jobSchedulerHandler.TriggerJob(request.VM.JobId, data);

                return new
                {

                }.Success();
            }
        }
    }
}
