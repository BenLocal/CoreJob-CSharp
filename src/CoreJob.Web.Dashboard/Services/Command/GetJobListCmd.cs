using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using CronExpressionDescriptor;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreJob.Web.Dashboard.Services.Command
{
    public class GetJobListCmd : IRequest<JsonEntityBase>
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public class GetJobListCmdRequest : IRequestHandler<GetJobListCmd, JsonEntityBase>
        {
            private readonly JobDbContext _dbContext;

            public GetJobListCmdRequest(JobDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<JsonEntityBase> Handle(GetJobListCmd request, CancellationToken cancellationToken)
            {
                var query = _dbContext.JobInfo.AsQueryable();

                var count = await query.CountAsync(cancellationToken).ConfigureAwait(false);
                var items = await query.Skip((request.PageIndex - 1) * request.PageSize)
                                        .Take(request.PageSize)
                                        .ToListAsync(cancellationToken)
                                        .ConfigureAwait(false);

                return new
                {
                    total = count,
                    rows = items.Select(x => new {
                        Cron = x.Cron,
                        CronDescription = ExpressionDescriptor.GetDescription(x.Cron),
                        Id = x.Id,
                        InTime = x.InTime,
                        Name = x.Name,
                        Status = x.Status,
                        UpdateTime = x.UpdateTime,
                        ExecutorId = x.ExecutorId,
                        ExecutorHandler = x.ExecutorHandler,
                        ExecutorParam = x.ExecutorParam
                    })
                }.Success();
            }
        }
    }
}
