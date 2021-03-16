using System.Collections.Generic;
using System.Reflection;
using CoreJob.Framework.Abstractions;
using CoreJob.Framework.Models;

namespace CoreJob.Server.Framework.Models
{
    public class CoreJobServerOptions : JobOptions
    {
        private readonly IList<IJobOptionsExtension> _extensions;

        internal IList<IJobOptionsExtension> Extensions => _extensions;

        public CoreJobServerOptions()
        {
            _extensions = new List<IJobOptionsExtension>();
        }


        public List<Assembly> JobAssemblies { get; set; } = new List<Assembly>();

        /// <summary>
        /// 默认清除3月之前的log信息
        /// </summary>
        public int ClearOldLogMonthTime { get; set; } = 3;

        public void RegisterExtension(IJobOptionsExtension extension)
        {
            _extensions.Add(extension);
        }
    }
}
