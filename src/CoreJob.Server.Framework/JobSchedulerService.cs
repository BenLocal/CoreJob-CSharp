using System;
using System.Threading;
using System.Threading.Tasks;
using CoreJob.Framework;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreJob.Dashboard.Services
{
    public class JobSchedulerService : IHostedService, IDisposable
    {
        private bool _started = false;

        private readonly IServiceProvider _provider;

        private readonly IJobSchedulerHandler _jobSchedulerHandler;

        public JobSchedulerService(IServiceProvider provider,
            IJobSchedulerHandler jobSchedulerHandler)
        {
            _provider = provider;
            _jobSchedulerHandler = jobSchedulerHandler;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                JobUtilExtensions.RunSync(async () => await StopAsync(new CancellationToken()));
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_started)
            {
                using (var scope = _provider.CreateScope())
                {
                    var _dbContext = scope.ServiceProvider.GetRequiredService<JobDbContext>();

                    var jobs = await _dbContext.JobInfo.ToListAsync();

                    if (jobs != null)
                    {
                        jobs.ForEach(async job =>
                        {
                            await _jobSchedulerHandler.AddOrUpdateJob(job, job.Status == 1);
                        });
                    }
                }

                await _jobSchedulerHandler.SystemScheduler();
                _started = true;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_started)
                {
                    await _jobSchedulerHandler.StopAll();
                }
            }
            finally
            {
                _started = false;
            }
        }
    }
}
