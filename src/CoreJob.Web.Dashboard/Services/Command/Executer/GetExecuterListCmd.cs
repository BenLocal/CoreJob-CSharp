using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Framework;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace CoreJob.Web.Dashboard.Services.Command.Executer
{
    public class GetExecuterListCmd : IRequest<JsonEntityBase>
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string SearchExecutorName { get; set; }

        public string SearchRegistryName { get; set; }

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

                var predicate = PredicateBuilder.True<JobExecuter>();
                if (request.SearchExecutorName.NotNullOrEmpty())
                {
                    predicate = predicate.And(x => x.Name.Contains(request.SearchExecutorName));
                }

                if (request.SearchRegistryName.NotNullOrEmpty())
                {
                    predicate = predicate.And(x => x.Name.Contains(request.SearchRegistryName));
                }

                var countFuture = query.Where(predicate).DeferredCount().FutureValue();
                var itemsFuture = query.Where(predicate).Include(x => x.RegistryHosts).OrderBy(x => x.Id)
                                        .Skip((request.PageIndex - 1) * request.PageSize)
                                        .Take(request.PageSize).Future();

                var total = await countFuture.ValueAsync();
                var items = await itemsFuture.ToListAsync();
                return new
                {
                    total = total,
                    rows = items.Select(x => new {
                        Id = x.Id,
                        InTime = x.InTime,
                        Name = x.Name,
                        RegistryHosts = x.RegistryHosts?.Select(x => x.Host),
                        RegistryKey = x.RegistryKey,
                        UpdateTime = x.UpdateTime
                    })
                }.Success();
            }
        }
    }
}
