using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Server.Framework.Store
{
    public class RegistryHost
    {
        public int Id { get; set; }

        public int ExecuterId { get; set; }

        public string Host { get; set; }

        public int Order { get; set; }

        public virtual JobExecuter JobExecuter { get; set; }
    }
}
