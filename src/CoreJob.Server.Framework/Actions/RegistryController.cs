using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreJob.Framework;
using CoreJob.Framework.Abstractions;
using CoreJob.Framework.Models.HttpAction;
using CoreJob.Framework.Models.Request;
using CoreJob.Framework.Models.Response;
using CoreJob.Server.Framework.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CoreJob.Server.Framework.Actions
{
    public class RegistryController : IJobController
    {
        private readonly JobDbContext _dbContext;

        public RegistryController(JobDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResponse> ActionAsync(HttpContext context)
        {
            var request = await context.Request.FromHttpRequestBody<RegistryRequest>();

            if (request == null || request.Key.NullOrEmpty() || request.Host.NullOrEmpty())
            {
                return await context.ResponseAsync("参数错误".Fail());
            }

            var host = request.Host.Trim();
            var db = await _dbContext.RegistryInfo.FirstOrDefaultAsync(x => x.Name == request.Key
                && x.Host.ToLower() == host.ToLower());

            if (db == null)
            {
                await _dbContext.RegistryInfo.AddAsync(new RegistryInfo()
                {
                    Host = host,
                    Name = request.Key,
                    InTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                });
            }

            var jobExecuter = _dbContext.JobExecuter.Include(x => x.RegistryHosts).FirstOrDefault(x => x.RegistryKey == request.Key && x.Auto);
            if (jobExecuter != null)
            {
                if (jobExecuter.RegistryHosts.IsNullOrEmptyList())
                {
                    await _dbContext.RegistryHost.AddAsync(new RegistryHost()
                    { 
                        ExecuterId = jobExecuter.Id,
                        Host = host,
                        Order = 0
                    });
                }
                else
                {
                    if (!jobExecuter.RegistryHosts.ToList().Exists(x => x.Host == host))
                    {
                        await _dbContext.RegistryHost.AddAsync(new RegistryHost()
                        {
                            ExecuterId = jobExecuter.Id,
                            Host = host,
                            Order = 0
                        });
                    }
                }
            }

            await _dbContext.SaveChangesAsync();

            return await context.ResponseAsync(string.Empty.Success());
        }
    }
}
