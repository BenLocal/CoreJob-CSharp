using System;
using System.Threading.Tasks;
using CoreJob.Client.Framework.Abstractions;
using CoreJob.Client.Framework.Models;
using CoreJob.Framework.Models.Response;
using Microsoft.Extensions.Logging;

namespace Core.Client.AspnetWeb
{
    [CoreJobHandler("demo")]
    public class DemoJobHandler : ICoreJobExecutorHandler
    {
        private readonly ILogger<DemoJobHandler> _logger;

        public DemoJobHandler(ILogger<DemoJobHandler> logger)
        {
            _logger = logger;
        }


        public Task<CoreBaseResponse<string>> Execute(JobExecuteContext context)
        {
            Console.WriteLine($"hello! :{context.JobParameter}");
            Console.WriteLine($"hello! :{context.LogId}");

            _logger.LogInformation("aaa");
            return Task.FromResult("ok".Success());
        }
    }
}
