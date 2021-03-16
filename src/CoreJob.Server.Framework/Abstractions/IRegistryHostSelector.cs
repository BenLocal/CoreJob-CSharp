using CoreJob.Framework.Models;
using CoreJob.Server.Framework.Store;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Server.Framework.Abstractions
{
    public interface IRegistryHostSelector
    {
        RegistryHost SelectHost(IEnumerable<RegistryHost> hosts, JobInfo jobInfo);
    }
}
