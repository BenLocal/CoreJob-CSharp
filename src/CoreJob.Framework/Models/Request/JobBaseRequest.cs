using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MessagePack;

namespace CoreJob.Framework.Models.Request
{
    [MessagePackObject]
    public class JobBaseRequest
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        [Key(0)]
        [JsonPropertyName("jobId")]
        public int JobId { get; set; }
    }
}
