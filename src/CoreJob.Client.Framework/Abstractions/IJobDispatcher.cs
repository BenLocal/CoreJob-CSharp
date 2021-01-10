using System.Threading.Tasks;
using CoreJob.Client.Framework.Models;

namespace CoreJob.Client.Framework.Abstractions
{
    public interface IJobDispatcher
    {
        Task Enqueue(JobMessage job);

        Task<bool> Exist(int jobId);

        Task<bool> Kill(int jobId, string killedReason);
    }
}
