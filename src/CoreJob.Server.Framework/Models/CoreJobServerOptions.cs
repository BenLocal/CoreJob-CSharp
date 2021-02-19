using System.Collections.Generic;
using System.Reflection;
using CoreJob.Framework.Models;

namespace CoreJob.Server.Framework.Models
{
    public class CoreJobServerOptions : JobOptions
    {
        public List<Assembly> JobAssemblies { get; set; } = new List<Assembly>();

        /// <summary>
        /// 默认清除3月之前的log信息
        /// </summary>
        public int ClearOldLogMonthTime { get; set; } = 3;
    }
}
