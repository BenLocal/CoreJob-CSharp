using System;

namespace CoreJob.Client.Framework.Models
{
    public class JobLoggerInfo
    {
        public int JobId { get; set; }
        public int JobLogId { get; set; }
        public DateTime InData { get; set; }
        public string Message { get; set; }
    }
}
