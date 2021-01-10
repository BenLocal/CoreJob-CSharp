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

            var jobExecuter = _dbContext.JobExecuter.FirstOrDefault(x => x.RegistryKey == request.Key && x.Auto);
            if (jobExecuter != null)
            {
                if (string.IsNullOrEmpty(jobExecuter.RegistryHosts))
                {
                    jobExecuter.RegistryHosts = host;
                }
                else
                {
                    var temp = jobExecuter.RegistryHosts.Split(",").ToList();
                    if (!temp.Contains(host))
                    {
                        temp.Add(host);
                        jobExecuter.RegistryHosts = string.Join(",", temp);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();

            return await context.ResponseAsync(string.Empty.Success());
        }
    }
}
