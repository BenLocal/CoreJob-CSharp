using System;
using CoreJob.Server.Framework.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CoreJob.Server.Store.SqlServer
{
    public class MysqlServerStoreProvider : IStoreProvider
    {
        private readonly IConfiguration _configuration;

        public MysqlServerStoreProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OptionsAction(DbContextOptionsBuilder options)
        {
            options.UseMySql(_configuration.GetConnectionString("CoreJobConnection"),
                    new MySqlServerVersion(new Version(8, 0, 21)),
                    b => b.MigrationsAssembly(typeof(MysqlServerStoreProvider).Assembly.GetName().Name));
        }
    }
}
