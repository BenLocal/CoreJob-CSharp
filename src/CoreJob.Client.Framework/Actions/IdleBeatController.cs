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
    public class IdleBeatController : IJobController
    {
        private readonly IJobDispatcher _dispatcher;

        public IdleBeatController(IJobDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task<IActionResponse> ActionAsync(HttpContext context)
        {
            var info = await context.Request.FromHttpRequestBody<JobBaseRequest>();
            if (info == null || info.JobId <= 0)
            {
                return await context.ResponseAsync( "参数错误".Fail());
            }

            var isRunning = await _dispatcher.Exist(info.JobId);
            if (isRunning)
            {
                return await context.ResponseAsync("任务执行中".Fail());
            }

            return await context.ResponseAsync(string.Empty.Success());
        }
    }
}
