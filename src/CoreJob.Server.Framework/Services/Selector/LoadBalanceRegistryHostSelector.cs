using CoreJob.Framework;
using CoreJob.Framework.HashAlgorithms;
using CoreJob.Framework.Models;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Store;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Server.Framework.Services.Selector
{
    public class LoadBalanceRegistryHostSelector : IRegistryHostSelector
    {
        //private static Lazy<ConcurrentDictionary<int, ConsistentHash<RegistryHost>>> _hashLazy =
        //    new Lazy<ConcurrentDictionary<int, ConsistentHash<RegistryHost>>>(() => new ConcurrentDictionary<int, ConsistentHash<RegistryHost>>(), true);

        public RegistryHost SelectHost(IEnumerable<RegistryHost> hosts, JobInfo jobInfo)
        {
            // Use cache the ConsistentHash object???
            // hosts's count is dynamically changing
            //if (!_hashLazy.Value.TryGetValue(jobInfo.Id, out var consistentHashing))
            //{
            //    consistentHashing = new ConsistentHash<RegistryHost>(new MurmurHash2HashAlgorithm(), 1024);
            //    consistentHashing.Initialize(hosts);

            //    _hashLazy.Value.TryAdd(jobInfo.Id, consistentHashing);
            //}

            var consistentHashing = new ConsistentHash<RegistryHost>(new MurmurHash2HashAlgorithm(), 1024);
            consistentHashing.Initialize(hosts);

            return consistentHashing.GetItemNode($"{jobInfo.Id}-{jobInfo.ExecutorHandler}-{jobInfo.ExecutorParam}");
        }
    }
}
