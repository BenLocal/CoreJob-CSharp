using System;
using CoreJob.Framework.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CoreJob.Client.Framework.Models
{
    public class CoreJobClientOptions : JobOptions
    {
        /// <summary>
        /// 最小30秒
        /// </summary>
        public int HeartbeatIntervalSecond { get; set; }

        /// <summary>
        /// 执行器名称 对应xxljob执行器配置的appName
        /// </summary>
        public string ExecutorAppName { get; set; }

        /// <summary>
        /// job调度Server地址
        /// </summary>
        public string AdminUrl { get; set; } = "https://localhost:44318/";

        /// <summary>
        /// 当前client节点地址
        /// </summary>
        public string ExecutorUrl { get; set; }

        /// <summary>
        /// 最少3次
        /// </summary>
        public int CallBackRetryCount { get; set; } = 3;

        public Action<FailedInfo> FailedCallback { get; set; }

        public ServiceLifetime HandlerServiceLifetime { get; set; } = ServiceLifetime.Scoped;
    }
}
