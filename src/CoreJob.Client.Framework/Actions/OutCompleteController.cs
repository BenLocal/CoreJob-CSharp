using System;
using System.Threading.Tasks;
using CoreJob.Framework.Abstractions;
using CoreJob.Framework.Models.HttpAction;
using Microsoft.AspNetCore.Http;

namespace CoreJob.Client.Framework.Actions
{
    public class OutCompleteController : IJobController
    {
        public Task<IActionResponse> ActionAsync(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
