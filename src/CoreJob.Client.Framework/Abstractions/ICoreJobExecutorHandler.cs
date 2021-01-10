using System.Threading.Tasks;
using CoreJob.Client.Framework.Models;
using CoreJob.Framework.Models.Response;

namespace CoreJob.Client.Framework.Abstractions
{
    public interface ICoreJobExecutorHandler
    {
        Task<CoreBaseResponse<string>> Execute(JobExecuteContext context);
    }
}
