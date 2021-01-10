using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using Newtonsoft.Json;

namespace CoreJob.Framework.Models.Request
{
    [MessagePackObject]
    public class JobBaseRequest
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        [Key(0)]
        [JsonProperty("jobId")]
        public int JobId { get; set; }
    }
}
