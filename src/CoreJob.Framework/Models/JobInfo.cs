using System;

namespace CoreJob.Framework.Models
{
    public class JobInfo
    {
        public int Id { get; set; }

        /// <summary>
        /// Cron表达式
        /// </summary>
        public string Cron { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 组
        /// </summary>
        public int ExecutorId { get; set; }

        public DateTime InTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public string ExecutorHandler { get; set; }

        public string ExecutorParam { get; set; }

        /// <summary>
        /// 0,停止，1运行
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CreateUser { get; set; }

        public int SelectorType { get; set; }

        /// <summary>
        /// 错误时发送邮件
        /// </summary>
        public string ErrorMailUser { get; set; }
    }
}
