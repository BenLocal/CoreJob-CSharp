using System;
using CoreJob.Server.Framework.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CoreJob.Server.Store.SqlServer
{
    public class SqlServerStoreProvider : IStoreProvider
    {
        private readonly IConfiguration _configuration;

        public SqlServerStoreProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OptionsAction(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_configuration.GetConnectionString("CoreJobConnection"),
                    b => b.MigrationsAssembly(typeof(SqlServerStoreProvider).Assembly.GetName().Name));
        }
    }
}
