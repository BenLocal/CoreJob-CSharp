using System;
using CoreJob.Framework;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreJob.Server.Store.Mysql
{
    public class MysqlServerStoreProvider : IStoreProvider
    {
        private readonly IConfiguration _configuration;

        private readonly ILoggerFactory _factory;

        public MysqlServerStoreProvider(IConfiguration configuration,
            ILoggerFactory factory)
        {
            _configuration = configuration;
            _factory = factory;
        }

        public void OptionsAction(DbContextOptionsBuilder options, StoreOptions storeOptions)
        {
            options.UseMySql(storeOptions.StoreConnectionStr,
                    new MySqlServerVersion(new Version(8, 0, 21)),
                    b => b.MigrationsAssembly(typeof(MysqlServerStoreProvider).Assembly.GetName().Name));
        }
    }
}
