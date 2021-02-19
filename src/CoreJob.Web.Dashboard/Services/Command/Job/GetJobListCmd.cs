using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Framework;
using CoreJob.Framework.Models;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using CronExpressionDescriptor;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace CoreJob.Web.Dashboard.Services.Command.Job
{
    public class GetJobListCmd : IRequest<JsonEntityBase>
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string SearchJobName { get; set; }

        public int SearchExecutorId { get; set; }

        public string SearchCreateUser { get; set; }

        public int SearchStatus { get; set; }

        public class GetJobListCmdRequest : IRequestHandler<GetJobListCmd, JsonEntityBase>
        {
            private readonly JobDbContext _dbContext;

            public GetJobListCmdRequest(JobDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<JsonEntityBase> Handle(GetJobListCmd request, CancellationToken cancellationToken)
            {
                var query = _dbContext.JobInfo.AsQueryable().AsNoTracking();

                var predicate = PredicateBuilder.True<JobInfo>();
                if (request.SearchJobName.NotNullOrEmpty())
                {
                    predicate = predicate.And(x => x.Name.Contains(request.SearchJobName));
                }

                if (request.SearchExecutorId > 0)
                {
                    predicate = predicate.And(x => x.ExecutorId == request.SearchExecutorId);
                }

                if (request.SearchStatus == 0 || request.SearchStatus == 1)
                {
                    predicate = predicate.And(x => x.Status == request.SearchStatus);
                }

                if (request.SearchCreateUser.NotNullOrEmpty())
                {
                    predicate = predicate.And(x => x.CreateUser == request.SearchCreateUser);
                }

                var countFuture = query.Where(predicate).DeferredCount().FutureValue();
                var itemsFuture = query.Where(predicate).OrderBy(x => x.Id).Skip((request.PageIndex - 1) * request.PageSize)
                                        .Take(request.PageSize).Future();

                var total = await countFuture.ValueAsync();
                var items = await itemsFuture.ToListAsync();
                return new
                {
                    total = total,
                    rows = items.Select(x => new {
                        Cron = x.Cron,
                        CronDescription = ExpressionDescriptor.GetDescription(x.Cron, new Options()
                        { 
                            Locale = "zh-cn"
                        }),
                        Id = x.Id,
                        InTime = x.InTime,
                        Name = x.Name,
                        Status = x.Status,
                        UpdateTime = x.UpdateTime,
                        ExecutorId = x.ExecutorId,
                        ExecutorHandler = x.ExecutorHandler,
                        ExecutorParam = x.ExecutorParam,
                        CreateUser = x.CreateUser
                    })
                }.Success();
            }
        }
    }
}
