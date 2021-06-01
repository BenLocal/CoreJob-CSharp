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
    public class DeleteExecuterHostCmd : IRequest<JsonEntityBase>
    {
        public int Id { get; set; }

        public class DeleteExecuterHostCmdRequest : IRequestHandler<DeleteExecuterHostCmd, JsonEntityBase>
        {
            private readonly JobDbContext _dbContext;

            public DeleteExecuterHostCmdRequest(JobDbContext dbContext)
            {
                _dbContext = dbContext;
            }


            public async Task<JsonEntityBase> Handle(DeleteExecuterHostCmd request, CancellationToken cancellationToken)
            {
                var dbValue = await _dbContext.RegistryHost.FindAsync(new RegistryHost() { Id = request.Id });
                if (dbValue != null)
                {
                    _dbContext.RegistryHost.Remove(dbValue);
                    await _dbContext.SaveChangesAsync();
                }

                return new
                {
                }.Success();
            }
        }
    }
}
