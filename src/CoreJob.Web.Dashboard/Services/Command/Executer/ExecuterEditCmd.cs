using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using MediatR;

namespace CoreJob.Web.Dashboard.Services.Command.Executer
{
    public class ExecuterEditCmd : IRequest<ExecuterViewModel>
    {
        public int ExecuterId { get; set; }

        public class ExecuterEditCmdRequest : IRequestHandler<ExecuterEditCmd, ExecuterViewModel>
        {
            private readonly JobDbContext _dbContext;

            public ExecuterEditCmdRequest(JobDbContext dbContext)
            {
                _dbContext = dbContext;
            }


            public async Task<ExecuterViewModel> Handle(ExecuterEditCmd request, CancellationToken cancellationToken)
            {
                var info = await _dbContext.JobExecuter.FindAsync(request.ExecuterId);

                return new ExecuterViewModel()
                {
                    IsNew = false,
                    ExecuterName = info.Name,
                    ExecuterId = info.Id,
                    RegistryHosts = info.RegistryHosts,
                    RegistryKey = info.RegistryKey
                };
            }
        }
    }
}
