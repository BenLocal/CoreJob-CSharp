using System.Threading.Tasks;
using CoreJob.Framework.Models.HttpAction;
using Microsoft.AspNetCore.Http;

namespace CoreJob.Framework.Abstractions
{
    public interface IJobController
    {
        Task<IActionResponse> ActionAsync(HttpContext context);
    }
}
