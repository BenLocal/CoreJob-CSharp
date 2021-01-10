using System.Threading.Tasks;
using CoreJob.Client.Framework.Abstractions;
using CoreJob.Client.Framework.Models;
using CoreJob.Framework;
using CoreJob.Framework.Abstractions;
using CoreJob.Framework.Models.HttpAction;
using CoreJob.Framework.Models.Request;
using CoreJob.Framework.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CoreJob.Client.Framework.Actions
{
    public class RunController : IJobController
    {
        private readonly ILogger<RunController> _logger;

        private readonly IJobDispatcher _dispatcher;

        public RunController(ILogger<RunController> logger,
            IJobDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }


        /// <summary>
        /// 说明：触发任务执行
        ///------
        ///地址格式：{执行器内嵌服务跟地址}/run
        ///Header：
        ///    XXL-JOB-ACCESS-TOKEN : {请求令牌}
        ///请求数据格式如下，放置在 RequestBody 中，JSON格式：
        ///    {
        ///        "jobId":1,                                  // 任务ID
        ///        "executorHandler":"demoJobHandler",         // 任务标识
        ///        "executorParams":"demoJobHandler",          // 任务参数
        ///        "executorBlockStrategy":"COVER_EARLY",      // 任务阻塞策略，可选值参考 com.xxl.job.core.enums.ExecutorBlockStrategyEnum
        ///        "executorTimeout":0,                        // 任务超时时间，单位秒，大于零时生效
        ///        "logId":1,                                  // 本次调度日志ID
        ///        "logDateTime":1586629003729,                // 本次调度日志时间
        ///        "glueType":"BEAN",                          // 任务模式，可选值参考 com.xxl.job.core.glue.GlueTypeEnum
        ///        "glueSource":"xxx",                         // GLUE脚本代码
        ///        "glueUpdatetime":1586629003727,             // GLUE脚本更新时间，用于判定脚本是否变更以及是否需要刷新
        ///        "broadcastIndex":0,                         // 分片参数：当前分片
        ///        "broadcastTotal":0                          // 分片参数：总分片
        ///    }
        ///响应数据格式：
        ///    {
        ///    "code": 200,      // 200 表示正常、其他失败
        ///      "msg": null       // 错误提示消息
        ///    }
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<IActionResponse> ActionAsync(HttpContext context)
        {
            var jobInfo = await context.Request.FromHttpRequestBody<RunRequest>();

            // 如果有积压任务，丢弃当前任务
            if (jobInfo.ExecutorBlockStrategy == JobConstant.EXECUTORBLOCKSTRATEGY_DISCARD_LATER)
            {
                if (await _dispatcher.Exist(jobInfo.JobId))
                {
                    return await context.ResponseAsync("终止任务[执行策略: DISCARD_LATER]".Fail());
                }

            }
            // 覆盖之前调度负载之前积压的任务
            else if (jobInfo.ExecutorBlockStrategy == JobConstant.EXECUTORBLOCKSTRATEGY_COVER_EARLY)
            {
                if (await _dispatcher.Exist(jobInfo.JobId))
                {
                    await _dispatcher.Kill(jobInfo.JobId, "终止任务[执行策略: COVER_EARLY]");
                }
            }

            await _dispatcher.Enqueue(new JobMessage(jobInfo));

            // action
            return await context.ResponseAsync(string.Empty.Success());
        }
    }
}
