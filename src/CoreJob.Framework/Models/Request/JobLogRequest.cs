using System.Text.Json.Serialization;
using MessagePack;

namespace CoreJob.Framework.Models.Request
{
    [MessagePackObject]
    public class JobLogRequest
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        [JsonPropertyName("logId")]
        [Key(0)]
        public int JobLogId { get; set; }

        /// <summary>
        /// 日志开始行号，滚动加载日志   // 从1开始
        /// </summary>
        [JsonPropertyName("fromLineNum")]
        [Key(1)]
        public int FromLineNum { get; set; }

        /// <summary>
        /// 本次调度日志时间
        /// </summary>
        [JsonPropertyName("logDateTim")]
        [Key(2)]
        public long LogDateTime { get; set; }
    }
}
