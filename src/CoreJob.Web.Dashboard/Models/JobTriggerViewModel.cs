using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreJob.Web.Dashboard.Models
{
    public class JobTriggerViewModel
    {
        public int JobId { get; set; }

        public string JobName { get; set; }

        public string ExecutorParam { get; set; }
    }
}
