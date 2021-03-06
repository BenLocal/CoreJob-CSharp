﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Framework.Abstractions;
using CoreJob.Server.Framework.Models;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Z.EntityFramework.Plus;

namespace CoreJob.Web.Dashboard.Services.Command.Job
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
                var executers = await _dbContext.JobExecuter.Select(x => new { value = x.Id, text = x.Name }).FromCacheAsync(new MemoryCacheEntryOptions()
                { 
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                });
                var seelctItems = new List<SelectListItem>()
                {
                    new SelectListItem("请选择执行器", "0")
                };

                if (executers != null)
                {
                    executers.ToList().ForEach(x => seelctItems.Add(new SelectListItem(x.text, x.value.ToString())));
                }

                vm.ExecutorItems = seelctItems;

                // 获取所有节点选择器类型
                var types = Enumeration.GetAll<SelectorType>();
                vm.SelectorTypeItems = types.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
                

                if (request.IsNew)
                {
                    return vm;
                }

                var jobInfo = await _dbContext.JobInfo.FindAsync(request.JobId);

                vm.ExecutorId = jobInfo.ExecutorId;

                // 当前节点选择器类型
                vm.SelectorType = jobInfo.SelectorType;

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
