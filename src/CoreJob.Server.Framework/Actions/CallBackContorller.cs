using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoreJob.Framework;
using CoreJob.Framework.Abstractions;
using CoreJob.Framework.Models.HttpAction;
using CoreJob.Framework.Models.Request;
using CoreJob.Framework.Models.Response;
using CoreJob.Server.Framework.Store;
using Microsoft.AspNetCore.Http;

namespace CoreJob.Server.Framework.Actions
{
    public class CallBackContorller : IJobController
    {
        private readonly JobDbContext _dbContext;

        public CallBackContorller(JobDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResponse> ActionAsync(HttpContext context)
        {
            var request = await context.Request.FromHttpRequestBody<List<CallBackResquest>>();

            if (request == null)
            {
                return await context.ResponseAsync("参数错误".Fail());
            }

            foreach (var log in request)
            {
                var jobLog = _dbContext.JobLog.Find(log.LogId);
                if (jobLog != null)
                {
                    jobLog.CompleteTime = log.LogDateTime.GetDateTime();
                    jobLog.CompleteCode = log.ResultCode;
                    jobLog.CompleteMsg = log.ResultMsg;
                }

                if (jobLog.CompleteCode == JobConstant.HTTP_SUCCESS_CODE)
                {
                    jobLog.Status = JobLogStatus.Success;
                }
                else if (jobLog.CompleteCode == JobConstant.HTTP_FAIL_CODE)
                {
                    jobLog.Status = JobLogStatus.Fail;
                }
            }

            await _dbContext.SaveChangesAsync();
            return await context.ResponseAsync(string.Empty.Success());
        }
    }
}
