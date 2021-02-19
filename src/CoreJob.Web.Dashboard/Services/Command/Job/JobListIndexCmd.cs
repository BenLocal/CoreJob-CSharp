using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Z.EntityFramework.Plus;

namespace CoreJob.Web.Dashboard.Services.Command.Job
{
    public class JobListIndexCmd : IRequest<JobListViewModel>
    {
        public class JobListIndexCmdRequest : IRequestHandler<JobListIndexCmd, JobListViewModel>
        {
            private readonly JobDbContext _dbContext;

            public JobListIndexCmdRequest(JobDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<JobListViewModel> Handle(JobListIndexCmd request, CancellationToken cancellationToken)
            {
                var vm = new JobListViewModel();

                // 获取所有Executor
                var executers = await _dbContext.JobExecuter.Select(x => new { value = x.Id, text = x.Name })
                    .FromCacheAsync(new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                });
                var seelctItems = new List<SelectListItem>()
                {
                    new SelectListItem("全部", "0")
                };

                if (executers != null)
                {
                    executers.ToList().ForEach(x => seelctItems.Add(new SelectListItem(x.text, x.value.ToString())));
                }

                vm.ExecutorItems = seelctItems;

                return vm;
            }
        }
    }
}
