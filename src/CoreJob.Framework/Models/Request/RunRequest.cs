using MessagePack;

namespace CoreJob.Framework.Models.Request
{
    [MessagePackObject]
    public class RunRequest
    {
        /// <summary>
        /// job id
        /// </summary>
        [Key(0)]
        public int JobId { get; set; }

        /// <summary>
        /// 任务标识 JobHandler
        /// </summary>
        [Key(1)]
        public string ExecutorHandler { get; set; }

        /// <summary>
        /// 任务参数
        /// </summary>
        [Key(2)]
        public string ExecutorParams { get; set; }

        /// <summary>
        /// 任务阻塞策略 SERIAL_EXECUTION=单机串行  DISCARD_LATER=丢弃后续调度  COVER_EARLY=覆盖之前调度 
        /// </summary>
        [Key(3)]
        public string ExecutorBlockStrategy { get; set; }

        /// <summary>
        /// 任务超时时间，单位秒，大于零时生效
        /// </summary>
        [Key(4)]
        public int ExecutorTimeout { get; set; }

        /// <summary>
        /// 本次调度日志ID
        /// </summary>
        [Key(5)]
        public int LogId { get; set; }

        /// <summary>
        /// 本次调度日志时间
        /// </summary>
        [Key(6)]
        public long LogDateTime { get; set; }

        /// <summary>
        /// 任务模式
        /// </summary>
        [Key(7)]
        public string GlueType { get; set; }

        /// <summary>
        /// GLUE脚本代码
        /// </summary>
        [Key(8)]
        public string GlueSource { get; set; }

        /// <summary>
        /// GLUE脚本更新时间，用于判定脚本是否变更以及是否需要刷新
        /// </summary>
        [Key(9)]
        public long GlueUpdatetime { get; set; }

        /// <summary>
        /// 分片参数：当前分片
        /// </summary>
        [Key(10)]
        public int BroadcastIndex { get; set; }

        /// <summary>
        /// 分片参数：总分片
        /// </summary>
        [Key(11)]
        public int BroadcastTotal { get; set; }
    }
}
