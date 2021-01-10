using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Server.Framework.Store
{
    public class JobLog
    {
        public int Id { get; set; }

        public int JobId { get; set; }

        public int ExecuterId { get; set; }

        public string ExecuterHost { get; set; }

        public string ExecuterHandler { get; set; }

        public string ExecuterParam { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? HandlerTime { get; set; }

        public int HandlerCode { get; set; }

        public string HandlerMsg { get; set; }

        public DateTime? CompleteTime { get; set; }

        public int CompleteCode { get; set; }

        public string CompleteMsg { get; set; }

        public JobLogStatus Status { get; set; }
    }

    public enum JobLogStatus
    { 
        Running,
        Success,
        Fail
    }
}
