using CoreJob.Framework.Abstractions;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CoreJob.Server.Store.Mysql
{
    public class MysqlJobOptionsExtension : IJobOptionsExtension
    {
        private readonly Action<StoreOptions> _configure;

        public MysqlJobOptionsExtension(Action<StoreOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            services.Configure(_configure);
            services.AddSingleton<IStoreProvider, MysqlServerStoreProvider>();
        }
    }
}
