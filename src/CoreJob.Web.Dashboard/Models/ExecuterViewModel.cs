using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreJob.Web.Dashboard.Models
{
    public class ExecuterViewModel
    {
        public bool IsNew { get; set; }

        public int ExecuterId { get; set; }

        public string ExecuterName { get; set; }

        public string RegistryKey { get; set; }

        public string RegistryHosts { get; set; }

        public bool Auto { get; set; } = true;
    }
}
