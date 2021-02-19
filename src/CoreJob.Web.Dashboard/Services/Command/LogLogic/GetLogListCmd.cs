using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreJob.Web.Dashboard.Services.Command.LogLogic
{
    public class GetLogListCmd : IRequest<JsonEntityBase>
    {
        public int JobId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public class GetLogListCmdRequest : IRequestHandler<GetLogListCmd, JsonEntityBase>
        {
            private readonly JobDbContext _dbContext;

            public GetLogListCmdRequest(JobDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<JsonEntityBase> Handle(GetLogListCmd request, CancellationToken cancellationToken)
            {
                var query = _dbContext.JobLog.AsQueryable();
                if (request.JobId > 0)
                {
                    query = query.Where(x => x.JobId == request.JobId);
                }

                var count = await query.CountAsync(cancellationToken).ConfigureAwait(false);
                var items = await query.OrderBy(x => x.Id).OrderByDescending(x => x.StartTime).Skip((request.PageIndex - 1) * request.PageSize)
                                        .Take(request.PageSize)
                                        .ToListAsync(cancellationToken)
                                        .ConfigureAwait(false);

                return new {
                    total = count,
                    rows = items
                }.Success();
            }
        }
    }
}
