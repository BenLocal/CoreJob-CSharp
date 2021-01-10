using System;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Framework.Models;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Helpers.Common;
using CoreJob.Web.Dashboard.Models;
using FluentValidation;
using MediatR;

namespace CoreJob.Web.Dashboard.Services.Command
{
    public class SaveJobCmd : IRequest<JsonEntityBase>
    {
        public JobViewModel VM { get; set; }
        public bool Trigger { get; set; }

        public class SaveJobCmdRequest : IRequestHandler<SaveJobCmd, JsonEntityBase>
        {
            private readonly JobDbContext _dbContext;

            private readonly IJobSchedulerHandler _jobSchedulerHandler;

            private readonly IValidationNotificationContext _validationNotificationContext;

            public SaveJobCmdRequest(JobDbContext dbContext,
                IJobSchedulerHandler jobSchedulerHandler,
                IValidationNotificationContext validationNotificationContext)
            {
                _dbContext = dbContext;
                _jobSchedulerHandler = jobSchedulerHandler;
                _validationNotificationContext = validationNotificationContext;
            }

            public async Task<JsonEntityBase> Handle(SaveJobCmd request, CancellationToken cancellationToken)
            {
                if (_validationNotificationContext.HasErrorNotifications)
                {
                    return _validationNotificationContext.GetErrorNotifications().Error();
                }

                var model = request.VM;
                if (model.IsNew || model.JobId <= 0)
                {
                    // add
                    //await _dbContext.JobInfo.FirstOrDefaultAsync(x => x.Name == model.JobName);
                    var job = new JobInfo()
                    {
                        Cron = model.Cron,
                        Status = request.Trigger ? 1 : 0,
                        ExecutorHandler = model.ExecutorHandler,
                        ExecutorParam = model.ExecutorParam,
                        InTime = DateTime.Now,
                        Name = model.JobName,
                        UpdateTime = DateTime.Now,
                        ExecutorId = model.ExecutorId,
                        CreateUser = model.CreateUser
                    };

                    await _dbContext.JobInfo.AddAsync(job);
                    await _jobSchedulerHandler.AddOrUpdateJob(job, request.Trigger);
                    await _dbContext.SaveChangesAsync();

                }
                else
                {
                    var job = await _dbContext.JobInfo.FindAsync(model.JobId);
                    job.Cron = model.Cron;
                    job.ExecutorHandler = model.ExecutorHandler;
                    job.ExecutorParam = model.ExecutorParam;
                    job.Name = model.JobName;
                    job.UpdateTime = DateTime.Now;
                    job.ExecutorId = model.ExecutorId;
                    job.Status = request.Trigger ? 1 : 0;

                    await _jobSchedulerHandler.AddOrUpdateJob(job, request.Trigger);
                    await _dbContext.SaveChangesAsync();
                }

                return new { }.Success();
            }
        }

        public class SaveJobCmdValidator : AbstractValidator<SaveJobCmd>
        {
            public SaveJobCmdValidator()
            {
                RuleFor(x => x.VM.JobName).NotEmpty().WithMessage("任务名称不能为空");
                RuleFor(x => x.VM.ExecutorId).GreaterThan(0).WithMessage("请选择注册器");
                RuleFor(x => x.VM.Cron).NotEmpty().WithMessage("Corn表达式不能为空");
                RuleFor(x => x.VM.ExecutorHandler).NotEmpty().WithMessage("任务执行器不能为空");
            }
        }
    }
}
