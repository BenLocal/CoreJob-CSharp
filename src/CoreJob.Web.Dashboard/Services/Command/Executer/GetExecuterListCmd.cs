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
    public class GetExecuterListCmd : IRequest<JsonEntityBase>
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public class GetLogListCmdRequest : IRequestHandler<GetExecuterListCmd, JsonEntityBase>
        {
            private readonly JobDbContext _dbContext;

            public GetLogListCmdRequest(JobDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<JsonEntityBase> Handle(GetExecuterListCmd request, CancellationToken cancellationToken)
            {
                var query = _dbContext.JobExecuter.AsQueryable();

                var count = await query.CountAsync(cancellationToken).ConfigureAwait(false);
                var items = await query.Skip((request.PageIndex - 1) * request.PageSize)
                                        .Take(request.PageSize)
                                        .ToListAsync(cancellationToken)
                                        .ConfigureAwait(false);

                return new
                {
                    total = count,
                    rows = items
                }.Success();
            }
        }
    }
}
