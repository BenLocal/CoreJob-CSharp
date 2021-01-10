using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoreJob.Framework.Models;
using CoreJob.Framework.Models.Response;

namespace CoreJob.Server.Framework.Abstractions
{
    public interface IServerSender
    {
        Task<ResponseContext<string>> RunAction(JobContext context);

        Task<ResponseContext<string>> BeatAction(string host);

        Task<ResponseContext<JobLogResponse>> LogAction(JobContext context);
    }
}
