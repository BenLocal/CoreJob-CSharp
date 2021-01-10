using System;

namespace CoreJob.Client.Framework.Models
{
    public class FailedInfo
    {
        public IServiceProvider ServiceProvider { get; set; }

        public Exception Exception { get; set; }

        public string Message { get; set; }

        public JobMessage Job { get; set; }
    }
}
