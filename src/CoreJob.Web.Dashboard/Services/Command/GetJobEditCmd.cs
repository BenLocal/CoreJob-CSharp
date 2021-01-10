using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoreJob.Web.Dashboard.Services.Command
{
    public class GetJobEditCmd : IRequest<JobViewModel>
    {
        public int JobId { get; set; }

        public bool IsNew { get; set; }

        public bool IsCopy { get; set; }

        public class GetJobEditCmdRequest : IRequestHandler<GetJobEditCmd, JobViewModel>
        {
            private readonly JobDbContext _dbContext;

            public GetJobEditCmdRequest(JobDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<JobViewModel> Handle(GetJobEditCmd request, CancellationToken cancellationToken)
            {
                var vm = new JobViewModel()
                {
                    IsNew = request.IsNew,
                    IsCopy = request.IsCopy,
                };

                // 获取所有Executor
                var executers = await _dbContext.JobExecuter.Select(x => new { value = x.Id, text = x.Name }).ToListAsync();
                var seelctItems = new List<SelectListItem>()
                {
                    new SelectListItem("请选择执行器", "0")
                };

                if (executers != null)
                {
                    executers.ForEach(x => seelctItems.Add(new SelectListItem(x.text, x.value.ToString())));
                }

                vm.ExecutorItems = seelctItems;

                if (request.IsNew)
                {
                    return vm;
                }

                var jobInfo = await _dbContext.JobInfo.FindAsync(request.JobId);

                vm.ExecutorId = jobInfo.ExecutorId;
                vm.Cron = jobInfo.Cron;
                vm.ExecutorHandler = jobInfo.ExecutorHandler;
                vm.ExecutorParam = jobInfo.ExecutorParam;

                if (request.IsCopy)
                {
                    vm.JobName = $"{jobInfo.Name} - Copy";
                }
                else
                {
                    vm.JobId = jobInfo.Id;
                    vm.JobName = jobInfo.Name;
                }

                return vm;
            }
        }
    }
}
