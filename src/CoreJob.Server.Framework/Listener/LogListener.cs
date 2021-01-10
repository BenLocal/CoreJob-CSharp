using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Framework;
using CoreJob.Framework.Models;
using CoreJob.Framework.Models.Response;
using CoreJob.Server.Framework.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CoreJob.Server.Framework.Listener
{
    /// <summary>
    /// exception: TriggerFired => VetoJobExecution => JobToBeExecuted => JobWasExecuted => TriggerComplete
    /// success: TriggerFired => VetoJobExecution => JobToBeExecuted => JobWasExecuted => TriggerComplete
    /// </summary>
    public class LogListener : IJobListener, ITriggerListener
    {
        public string Name { get; set; } = "LogListener";

        //private readonly JobDbContext _dbContext;

        private readonly ILogger<LogListener> _logger;

        private readonly IServiceProvider _provider;

        public LogListener(IServiceProvider provider,
            ILogger<LogListener> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        /// <summary>
        /// job被否决
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"JobExecutionVetoed: {DateTime.Now}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// job开始执行
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            var jobInfo = context.Trigger.JobDataMap.GetMapData<JobInfo>();

            using (var scope = _provider.CreateScope())
            using (var _dbContext = scope.ServiceProvider.GetRequiredService<JobDbContext>())
            {
                var jobExecutor = await _dbContext.JobExecuter.FindAsync(jobInfo.ExecutorId);
                if (jobExecutor == null || jobExecutor.RegistryHosts.NullOrEmpty())
                {
                    _logger.LogError("没有找到执行器，任务中断");
                    return;
                }
                // 找到合适的执行host
                var host = jobExecutor.RegistryHosts.Split(",").FirstOrDefault();

                context.Trigger.JobDataMap.SetMapData(jobExecutor);

                var logInfo = new JobLog()
                {
                    StartTime = DateTime.Now,
                    JobId = jobInfo.Id,
                    ExecuterHandler = jobInfo.ExecutorHandler,
                    ExecuterParam = jobInfo.ExecutorParam,
                    ExecuterId = jobInfo.ExecutorId,
                    ExecuterHost = host,
                    Status = JobLogStatus.Running
                };

                await _dbContext.JobLog.AddAsync(logInfo);
                await _dbContext.SaveChangesAsync();

                context.Trigger.JobDataMap.Put(JobConstant.MAP_DATA_EXECUTER_HOST_KEY, host);
                context.Trigger.JobDataMap.SetMapData(logInfo);
            }    
        }

        /// <summary>
        /// job执行完成
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jobException"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            var logInfo = context.Trigger.JobDataMap.GetMapData<JobLog>();
            logInfo.HandlerTime = DateTime.Now;
            // 错误
            if (jobException != null)
            {
                logInfo.Status = JobLogStatus.Fail;
                logInfo.HandlerCode = JobConstant.HTTP_FAIL_CODE;
                logInfo.HandlerMsg = $@"执行出现异常：{jobException.Message}<br>异常堆栈: {jobException.StackTrace}";
                return;
            }
            else
            {
                var response = context.Trigger.JobDataMap.GetMapData<ResponseContext<string>>();
                logInfo.HandlerCode = response.Code;

                if (logInfo.HandlerCode == JobConstant.HTTP_FAIL_CODE)
                {
                    logInfo.Status = JobLogStatus.Fail;
                    logInfo.HandlerMsg = $@"执行失败<br>StatusCode: {response.StatusCode}<br>错误信息: {response.Msg}";
                }
                else 
                {
                    logInfo.HandlerMsg = $@"执行成功";
                }
                
            }
            using (var scope = _provider.CreateScope())
            using (var _dbContext = scope.ServiceProvider.GetRequiredService<JobDbContext>())
            {
                _dbContext.Update(logInfo);
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Trigger完成时调用
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="triggerInstructionCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"TriggerComplete: {DateTime.Now}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// job执行时调用
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"TriggerFired: {DateTime.Now}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 错过触发时调用(例：线程不够用的情况下)
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"TriggerMisfired: {DateTime.Now}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Trigger触发后，job执行时调用本方法。true即否决，job后面不执行。
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }
    }
}
