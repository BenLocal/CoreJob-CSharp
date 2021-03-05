using System;
using CoreJob.Server.Framework.Abstractions;
using CoreJob.Server.Framework.Models;
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

        public void OptionsAction(DbContextOptionsBuilder options, StoreOptions storeOptions)
        {
            options.UseSqlServer(storeOptions.StoreConnectionStr,
                    b => b.MigrationsAssembly(typeof(SqlServerStoreProvider).Assembly.GetName().Name));
        }
    }
}
