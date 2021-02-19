using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Models;
using CoreJob.Server.Framework.Store;
using Quartz;
using Z.EntityFramework.Plus;

namespace CoreJob.Server.Framework.Jobs
{
    public class ClearLogsJob : IJob
    {
        private readonly JobDbContext _dbContext;

        private readonly CoreJobServerOptions _options;

        public ClearLogsJob(JobDbContext dbContext, CoreJobServerOptions options)
        {
            _dbContext = dbContext;
            _options = options;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _dbContext.JobLog.Where(x => x.StartTime < DateTime.Today.AddMonths(-_options.ClearOldLogMonthTime)).DeleteAsync();
        }
    }
}
