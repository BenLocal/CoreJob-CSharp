using System;
using System.Linq;
using CoreJob.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreJob.Server.Framework.Store
{
    public static class DbContextExtension
    {
        public static IHost EnsureCreatedDB(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<JobDbContext>();
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate(); //执行迁移
                }

                // 初始化用户
                if (!context.User.Any())
                {
                    var newUser = new DashboardUser()
                    {
                        DisplayName = "admin",
                        Disabled = false,
                        HashPassword = "admin".MD5Encrypt(),
                        InTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        Name = "admin",
                    };
                    context.User.Add(newUser);
                }

                context.SaveChanges();
            }
            return host;
        }
    }
}
