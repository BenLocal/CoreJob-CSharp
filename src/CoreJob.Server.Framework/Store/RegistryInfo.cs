using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Server.Framework.Store
{
    public class RegistryInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Host { get; set; }

        public DateTime InTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
