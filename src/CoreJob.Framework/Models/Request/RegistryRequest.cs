using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace CoreJob.Framework.Models.Request
{
    [MessagePackObject]
    public class RegistryRequest
    {
        /// <summary>
        /// 执行器AppName
        /// </summary>
        [Key(0)]
        public string Key { get; set; }

        /// <summary>
        /// 执行器地址
        /// </summary>
        [Key(1)]
        public string Host { get; set; }
    }
}
