using CoreJob.Server.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.Server.Store.Mysql
{
    public static class JobOptionsExtensions
    {
        public static CoreJobServerOptions UseMysql(this CoreJobServerOptions options, Action<StoreOptions> configure)
        {
            options.RegisterExtension(new MysqlJobOptionsExtension(configure));

            return options;
        }
    }
}
