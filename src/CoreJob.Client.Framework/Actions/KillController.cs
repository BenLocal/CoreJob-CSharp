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
    public class KillController : IJobController
    {
        private readonly IJobDispatcher _dispatcher;

        public KillController(IJobDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task<IActionResponse> ActionAsync(HttpContext context)
        {
            var info = await context.Request.FromHttpRequestBody<JobBaseRequest>();
            if (info == null || info.JobId <= 0)
            {
                return await context.ResponseAsync("参数错误".Fail());
            }

            if (await _dispatcher.Kill(info.JobId, "终止任务[调度中心主动停止任务]"))
            {
                return await context.ResponseAsync(string.Empty.Success());
            }

            return await context.ResponseAsync("终止任务失败".Fail());
        }
    }
}
