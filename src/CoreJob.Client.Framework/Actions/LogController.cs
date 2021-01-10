using System.Text;
using System.Threading.Tasks;
using CoreJob.Client.Framework.Abstractions;
using CoreJob.Framework;
using CoreJob.Framework.Abstractions;
using CoreJob.Framework.Models.HttpAction;
using CoreJob.Framework.Models.Request;
using CoreJob.Framework.Models.Response;
using Microsoft.AspNetCore.Http;

namespace CoreJob.Client.Framework.Actions
{
    public class LogController : IJobController
    {
        private readonly IJobLoggerStore _store;

        private const int Default_Limit = 20;

        public LogController(IJobLoggerStore store)
        {
            _store = store;
        }

        public async Task<IActionResponse> ActionAsync(HttpContext context)
        {
            var info = await context.Request.FromHttpRequestBody<JobLogRequest>();
            if (info == null || info.JobLogId <= 0)
            {
                return await context.ResponseAsync("参数错误".Fail());
            }

            (var logs, var totalCount) = await _store.FilterJobList(info.FromLineNum - 1, Default_Limit, info.JobLogId);

            StringBuilder sb = new StringBuilder();
            foreach (var log in logs)
            {
                sb.AppendLine($"[{log.InData.ToString("yyyy-MM-dd HH:mm:ss")}],{log.Message}");
            }

            // 从1开始
            var endLineNum = info.FromLineNum + logs.Count;

            var res = new JobLogResponse() {
                // 本次请求，日志开始行数
                FromLineNum = info.FromLineNum,
                // 本次请求，日志结束行号
                ToLineNum = endLineNum,
                // 本次请求日志内容
                LogContent = sb.ToString(),
                // 日志是否全部加载完
                IsEnd = totalCount <= endLineNum
            }.Success();
          

            return await context.ResponseAsync(res);
        }
    }
}
