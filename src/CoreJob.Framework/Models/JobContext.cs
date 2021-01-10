using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Framework.Models
{
    public class JobContext
    {
        public JobContext(JobInfo job)
        {
            Detail = job;
            Id = job.Id;
        }

        public JobContext()
        { 
            
        }

        public JobInfo Detail { get; }

        public int Id { get; set; }

        public int LogId { get; set; }

        public long LogDateTime { get; set; }

        public string ClientHostUrl { get; set; }

        public int LogFromLineNum { get; set; }
    }
}
