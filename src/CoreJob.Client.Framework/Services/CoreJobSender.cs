using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreJob.Client.Framework.Abstractions;
using CoreJob.Client.Framework.Models;
using CoreJob.Framework;
using CoreJob.Framework.Models.Request;
using CoreJob.Framework.Models.Response;
using Microsoft.Extensions.Logging;

namespace CoreJob.Client.Framework.Services
{
    public class CoreJobSender : IJobSender
    {
        private readonly ICoreJobServicesProvider<ICoreJobExecutorHandler> _handlerProvider;

        private readonly ILogger<CoreJobSender> _logger;

        private readonly ICoreJobExecutor _adminExecutor;

        private readonly CoreJobClientOptions _options;

        public CoreJobSender(ICoreJobServicesProvider<ICoreJobExecutorHandler> handlerProvider,
            ILogger<CoreJobSender> logger,
            ICoreJobExecutor adminExecutor,
            CoreJobClientOptions options)
        {
            _handlerProvider = handlerProvider;
            _logger = logger;
            _adminExecutor = adminExecutor;
            _options = options;
        }

        public async Task CallBack(List<JobMessage> jobs)
        {
            if (jobs == null || jobs.Count <= 0)
            {
                return;
            }

            bool retry;
            var callBackResult = new CallBackResult();
            do
            {
                var executedResult = await CallBackWithoutRetryAsync(callBackResult, jobs);
                var result = executedResult.Item1;
                if (result)
                {
                    return;
                }
                retry = executedResult.Item2;
            } while (retry);
        }

        private async Task<(bool, bool)> CallBackWithoutRetryAsync(CallBackResult callBackResult, List<JobMessage> jobs)
        {
            var result = await _adminExecutor.CallBackExecutor(jobs.Select(x => new CallBackResquest()
            {
                ResultCode = x.CallBackCode,
                LogDateTime = DateTime.Now.GetTimeStamp(),
                LogId = x.LogId,
                ResultMsg = x.Reason
            }).ToList());

            var needRetry = false;
            if (!result)
            {
                var retries = ++callBackResult.Retries;
                var retryCount = Math.Min(_options.CallBackRetryCount, 3);
                if (retries >= retryCount)
                {
                    needRetry = false;
                    _logger.LogError($"corejob任务结果回调失败,已超过重试次数");
                }
                else
                {
                    needRetry = true;
                    _logger.LogError($"corejob任务结果回调失败,logIds:{string.Join(",", jobs.Select(x => x.Id))},retries:{retries}");
                }
            }

            return (result, needRetry);
        }

        public async Task<CoreBaseResponse<string>> Execute(JobMessage job)
        {
            var handler = await _handlerProvider.GetInstanceAsync(job.ExecutorHandler);
            if (handler == null)
            {
                return $"没有找到名为{job.ExecutorHandler}的JobHandler".Fail();
            }

            // if killed
            if (job.Status == JobStatus.Killed)
            {
                return $"任务已被终止,id:{job.Id}, reason: {job.Reason}, logId: {job.LogId}".Fail();
            }

            job.Run();

            try
            {
                return await handler.Execute(new JobExecuteContext(job.Id, job.LogId, job.ExecutorParams));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "corejob执行任务错误");
                return e.Message.Fail();
            }
        }
    }
}
