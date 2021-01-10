using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CoreJob.Web.Dashboard.Models;
using CoreJob.Server.Framework.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CoreJob.Web.Dashboard.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        private readonly JobDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger,
            JobDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var jobCount = await _dbContext.JobInfo.CountAsync();
            var executerCount = await _dbContext.JobExecuter.CountAsync();
            var runTimes = await _dbContext.JobLog.CountAsync();

            var chatData = await _dbContext.JobLog.Where(x => x.StartTime > DateTime.Now.AddDays(-7)).GroupBy(x => x.StartTime.Date).Select(x => new
            {
                time = x.Key,
                success = x.Count(x => x.Status == JobLogStatus.Success),
                fail = x.Count(x => x.Status == JobLogStatus.Fail),
                running = x.Count(x => x.Status == JobLogStatus.Running)
            }).OrderBy(x => x.time).ToListAsync();

            var model = new DashboardViewModel()
            {
                JobCount = jobCount,
                ExecuterCount = executerCount,
                RunTimes = runTimes,
                HasData = (chatData != null && chatData.Any()) ? "yes" : string.Empty ,
                DataTimeList = chatData.Select(x => x.time.ToString("yyyy-MM-dd")).ToList(),
                SuccessCountList = chatData.Select(x => x.success).ToList(),
                FailCountList = chatData.Select(x => x.fail).ToList(),
                RunningCountList = chatData.Select(x => x.running).ToList(),
            };
            return View(model);
        }
    }
}
