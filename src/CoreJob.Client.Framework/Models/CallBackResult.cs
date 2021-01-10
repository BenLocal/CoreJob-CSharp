using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Client.Framework.Models
{
    public class CallBackResult
    {
        public DateTime? ExpiresAt { get; set; }

        public int Retries { get; set; }
    }
}
