using System.Threading.Tasks;
using CoreJob.Framework;
using CoreJob.Framework.Abstractions;
using CoreJob.Framework.Models.HttpAction;
using CoreJob.Framework.Models.Response;
using Microsoft.AspNetCore.Http;

namespace CoreJob.Client.Framework.Actions
{
    public class BeatController : IJobController
    {
        public async Task<IActionResponse> ActionAsync(HttpContext context)
        {
            return await context.ResponseAsync(string.Empty.Success());
        }
    }
}
