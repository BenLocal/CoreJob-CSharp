using System;
using System.Linq;
using CoreJob.Framework.Models;
using CoreJob.Server.Framework.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Z.EntityFramework.Plus;

namespace CoreJob.Framework.Test
{
    public class ClearLogsTest
    {
        public static readonly ILoggerFactory TestLoggerFactory =
            LoggerFactory.Create(builder => builder.AddConsole());

        protected ServiceProvider provider { get; set; }

        [SetUp]
        public void Setup()
        {
            IServiceCollection service = new ServiceCollection();
            service.AddDbContext<JobDbContext>(options =>
            {
                //options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=core_job;Trusted_Connection=True;ConnectRetryCount=0");

                options.UseLoggerFactory(TestLoggerFactory).EnableSensitiveDataLogging();
            });

            provider = service.BuildServiceProvider();
        }

        [Test]
        public void ClearLogs()
        {
            var _dbContext = provider.GetService<JobDbContext>();

            //_dbContext.JobLog.FromCache().ToList();

            //var count = _dbContext.JobLog.Where(x => x.StartTime < DateTime.Today.AddMonths(-3)).Delete();

            _dbContext.Database.ExecuteSqlRaw($"DELETE FROM {nameof(JobLog)} WHERE {nameof(JobLog.StartTime)} < {0}", DateTime.Today.AddMonths(-3));

        }
    }
}
