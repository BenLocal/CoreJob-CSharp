using System;
using System.Linq;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Store;
using CoreJob.Web.Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;

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
            var jobCountQuery = _dbContext.JobInfo.DeferredCount().FutureValue();
            var executerCountQuery = _dbContext.JobExecuter.DeferredCount().FutureValue();
            var runTimesQuery = _dbContext.JobLog.DeferredCount().FutureValue();

            var chatDataQuery = _dbContext.JobLog.AsNoTracking().Where(x => x.StartTime > DateTime.Now.AddDays(-7)).GroupBy(x => x.StartTime.Date).Select(x => new
            {
                time = x.Key,
                success = x.Count(x => x.Status == JobLogStatus.Success),
                fail = x.Count(x => x.Status == JobLogStatus.Fail),
                running = x.Count(x => x.Status == JobLogStatus.Running)
            }).OrderBy(x => x.time).Future();

            var chatData = await chatDataQuery.ToListAsync();
            var model = new DashboardViewModel()
            {
                JobCount = await jobCountQuery.ValueAsync(),
                ExecuterCount = await executerCountQuery.ValueAsync(),
                RunTimes = await runTimesQuery.ValueAsync(),
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
