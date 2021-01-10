using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Store;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace CoreJob.Server.Framework.Jobs
{
    public class RegistryHealthCheckJob : IJob
    {
        private readonly JobDbContext _dbContext;

        private readonly IServerSender _serverSender;

        public RegistryHealthCheckJob(JobDbContext dbContext,
            IServerSender serverSender)
        {
            _dbContext = dbContext;
            _serverSender = serverSender;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var infoList = await _dbContext.RegistryInfo.Skip(0).Take(100).ToListAsync();
            if (infoList == null)
            {
                return;
            }

            foreach (var registry in infoList)
            {
                var response = await _serverSender.BeatAction(registry.Host);
                if (!response.Health())
                {
                    // remove
                    _dbContext.RegistryInfo.Remove(registry);

                    var jobExecuter = _dbContext.JobExecuter.FirstOrDefault(x => x.RegistryKey == registry.Name && x.Auto);
                    if (jobExecuter != null && !string.IsNullOrEmpty(jobExecuter.RegistryHosts))
                    {
                        var temp = jobExecuter.RegistryHosts.Split(",").ToList();
                        if (temp.Contains(registry.Host))
                        {
                            temp.Remove(registry.Host);
                            jobExecuter.RegistryHosts = string.Join(",", temp);
                        }
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
