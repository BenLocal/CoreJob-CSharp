using CoreJob.Framework.Models;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreJob.Server.Framework.Services.Selector
{
    public class DefaultRegistryHostSelector : IRegistryHostSelector
    {
        public RegistryHost SelectHost(IEnumerable<RegistryHost> hosts, JobInfo jobInfo)
        {
            return hosts.FirstOrDefault();
        }
    }
}
