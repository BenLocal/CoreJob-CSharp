using System.Threading;
using System.Threading.Tasks;
using CoreJob.Framework;
using CoreJob.Framework.Models;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using MediatR;

namespace CoreJob.Web.Dashboard.Services.Command.Log
{
    public class GetLogInfoCmd : IRequest<JsonEntityBase>
    {
        public int LogId { get; set; }

        public int LogFromLineNum { get; set; } = 1;

        public class GetLogInfoCmdRequest : IRequestHandler<GetLogInfoCmd, JsonEntityBase>
        {
            private readonly IServerSender _serverSender;

            private readonly JobDbContext _dbContext;

            public GetLogInfoCmdRequest(IServerSender serverSender, JobDbContext dbContext)
            {
                _serverSender = serverSender;
                _dbContext = dbContext;
            }

            public async Task<JsonEntityBase> Handle(GetLogInfoCmd request, CancellationToken cancellationToken)
            {
                var logInfo = await _dbContext.JobLog.FindAsync(request.LogId);

                var result = await _serverSender.LogAction(new JobContext()
                {
                    LogId = request.LogId,
                    LogFromLineNum = request.LogFromLineNum,
                    ClientHostUrl = logInfo.ExecuterHost
                });

                if (result.StatusCode == System.Net.HttpStatusCode.OK && result.Code == JobConstant.HTTP_SUCCESS_CODE)
                {
                    return result.Content.Success(); 
                }

                return "没有数据".Error();
            }
        }
    }
}
