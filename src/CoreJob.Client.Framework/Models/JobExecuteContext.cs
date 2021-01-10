using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Client.Framework.Models
{
    public class JobExecuteContext
    {
        public int JobId { get; private set; }

        /// <summary>
        /// 本次调度日志ID
        /// </summary>
        public int LogId { get; private set; }

        public string JobParameter { get; private set; }

        public JobExecuteContext(int id, int logId, string jobParameter)
        {
            this.JobId = id;
            this.LogId = logId;
            this.JobParameter = jobParameter;
        }
    }
}
