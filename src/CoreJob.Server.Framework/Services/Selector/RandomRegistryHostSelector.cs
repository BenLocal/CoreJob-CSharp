using CoreJob.Framework.Models;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreJob.Server.Framework.Services.Selector
{
    public class RandomRegistryHostSelector : IRegistryHostSelector
    {
        private readonly static Random random = new Random();

        public RegistryHost SelectHost(IEnumerable<RegistryHost> hosts, JobInfo jobInfo)
        {
            int num = random.Next(0, hosts.Count());
            return hosts.ElementAtOrDefault(num);
        }
    }
}
