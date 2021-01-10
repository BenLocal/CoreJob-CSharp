using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Server.Framework.Store
{
    public class JobExecuter
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string RegistryKey { get; set; }

        public string RegistryHosts { get; set; }

        public bool Auto { get; set; }

        public DateTime InTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
