using System.Collections.Generic;
using System.Threading.Tasks;
using CoreJob.Client.Framework.Models;
using CoreJob.Framework.Models.Response;

namespace CoreJob.Client.Framework.Abstractions
{
    public interface IJobSender
    {
        Task<CoreBaseResponse<string>> Execute(JobMessage job);

        Task CallBack(List<JobMessage> jobs);
    }
}
