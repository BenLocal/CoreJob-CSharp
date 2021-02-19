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
    public class RemoveRegistryController : IJobController
    {
        private readonly JobDbContext _dbContext;

        public RemoveRegistryController(JobDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResponse> ActionAsync(HttpContext context)
        {
            var request = await context.Request.FromHttpRequestBody<RemoveRegistryRequest>();

            if (request == null)
            {
                return await context.ResponseAsync("参数错误".Fail());
            }
            var host = request.Host.Trim();
            var db = _dbContext.RegistryInfo.FirstOrDefault(x => x.Name == request.Key
                && x.Host.ToLower() == host.ToLower());

            if (db != null)
            {
                _dbContext.RegistryInfo.Remove(db);

                var jobExecuter = _dbContext.JobExecuter.Include(x => x.RegistryHosts).FirstOrDefault(x => x.RegistryKey == db.Name && x.Auto);
                if (jobExecuter != null && jobExecuter.RegistryHosts.NotNullOrEmptyList())
                {
                    foreach (var item in jobExecuter.RegistryHosts)
                    {
                        if (item.Host == host)
                        {
                            jobExecuter.RegistryHosts.Remove(item);
                            break;
                        }
                    }
                }
            }

            await _dbContext.SaveChangesAsync();

            return await context.ResponseAsync(string.Empty.Success());
        }
    }
}
