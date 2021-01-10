using System.Collections.Generic;
using System.Threading.Tasks;
using CoreJob.Client.Framework.Models;

namespace CoreJob.Client.Framework.Abstractions
{
    public interface IJobLoggerStore
    {
        Task Save(JobLoggerInfo entry);

        Task<(List<JobLoggerInfo>, int totalCount)> FilterJobList(int index, int limit, int jobLogId);
    }
}
