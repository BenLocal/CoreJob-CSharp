using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
                var info = await _dbContext.JobExecuter.Include(x => x.RegistryHosts).FirstOrDefaultAsync(x => x.Id == request.ExecuterId);

                return new ExecuterViewModel()
                {
                    IsNew = false,
                    ExecuterName = info.Name,
                    ExecuterId = info.Id,
                    RegistryHosts = info.RegistryHosts?.Select(x => new RegistryHostsItem()
                    { 
                        Id = x.Id,
                        Url = x.Host
                    }).ToList(),
                    RegistryKey = info.RegistryKey
                };
            }
        }
    }
}
