using CoreJob.Framework.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreJob.Server.Framework.Store
{
    public class JobDbContext : DbContext
    {
        private readonly string _connectionString;

        public JobDbContext(DbContextOptions<JobDbContext> options) : base(options)
        { 
        }

        public JobDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<JobInfo> JobInfo { get; set; }

        public DbSet<RegistryInfo> RegistryInfo { get; set; }

        public DbSet<JobExecuter> JobExecuter { get; set; }

        public DbSet<JobLog> JobLog { get; set; }

        public DbSet<DashboardUser> User { get; set; }

        public DbSet<RegistryHost> RegistryHost { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            AddConfiguration(modelBuilder);
        }

        private void AddConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobInfo>(entity =>
            {
                entity.ToTable("job_info");
                entity.HasKey(x => x.Id).HasName("id");
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(x => x.Cron).HasColumnName("cron").HasMaxLength(50);
                entity.Property(x => x.InTime).HasColumnName("in_time");
                entity.Property(x => x.UpdateTime).HasColumnName("update_time");
                entity.Property(x => x.ExecutorId).HasColumnName("executor_id");
                entity.Property(x => x.ExecutorHandler).HasColumnName("executor_handler").HasMaxLength(50);
                entity.Property(x => x.ExecutorParam).HasColumnName("executor_param").HasMaxLength(1000);
                entity.Property(x => x.Status).HasColumnName("status");
                entity.Property(x => x.CreateUser).HasColumnName("create_user");

                entity.HasIndex(x => x.Name);
            });

            modelBuilder.Entity<RegistryInfo>(entity =>
            {
                entity.ToTable("registry_info");
                entity.HasKey(x => x.Id).HasName("id");
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(x => x.Host).HasColumnName("host").HasMaxLength(100).IsRequired();
                entity.Property(x => x.InTime).HasColumnName("in_time");
                entity.Property(x => x.UpdateTime).HasColumnName("update_time");

                entity.HasIndex(x => x.Name);
                entity.HasIndex(x => x.Host);
            });

            modelBuilder.Entity<JobExecuter>(entity =>
            {
                entity.ToTable("job_executer");
                entity.HasKey(x => x.Id).HasName("id");
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(x => x.RegistryKey).HasColumnName("registry_key").HasMaxLength(100).IsRequired();
                //entity.Property(x => x.RegistryHosts).HasColumnName("registry_hosts").HasMaxLength(1000);
                entity.Property(x => x.InTime).HasColumnName("in_time");
                entity.Property(x => x.UpdateTime).HasColumnName("update_time");
                entity.Property(x => x.Auto).HasColumnName("auto");

                entity.HasIndex(x => x.Name);
                entity.HasIndex(x => x.RegistryKey);
            });

            modelBuilder.Entity<JobLog>(entity =>
            {
                entity.ToTable("job_log");
                entity.HasKey(x => x.Id).HasName("id");
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(x => x.JobId).HasColumnName("job_id").IsRequired();
                entity.Property(x => x.ExecuterId).HasColumnName("executer_id").IsRequired();
                entity.Property(x => x.ExecuterHost).HasColumnName("executer_host").HasMaxLength(1000);
                entity.Property(x => x.ExecuterHandler).HasColumnName("executer_handler").HasMaxLength(100);
                entity.Property(x => x.ExecuterParam).HasColumnName("executer_param").HasMaxLength(100);
                entity.Property(x => x.StartTime).HasColumnName("start_time");
                entity.Property(x => x.HandlerTime).HasColumnName("handler_time").IsRequired(false);
                entity.Property(x => x.HandlerCode).HasColumnName("handler_code");
                entity.Property(x => x.HandlerMsg).HasColumnName("handler_msg").HasMaxLength(1000);
                entity.Property(x => x.CompleteTime).HasColumnName("complete_time").IsRequired(false);
                entity.Property(x => x.CompleteCode).HasColumnName("complete_code");
                entity.Property(x => x.CompleteMsg).HasColumnName("complete_msg").HasMaxLength(1000);
                entity.Property(x => x.Status).HasColumnName("status");

                entity.HasIndex(x => x.JobId);
                entity.HasIndex(x => x.ExecuterId);
            });

            modelBuilder.Entity<DashboardUser>(entity =>
            {
                entity.ToTable("dashboard_user");
                entity.HasKey(x => x.Id).HasName("id");
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(x => x.HashPassword).HasColumnName("password").HasMaxLength(500).IsRequired();
                entity.Property(x => x.Disabled).HasColumnName("is_disabled").HasDefaultValue(false);
                entity.Property(x => x.InTime).HasColumnName("in_time");
                entity.Property(x => x.UpdateTime).HasColumnName("update_time");
                entity.Property(x => x.DisplayName).HasColumnName("display_name");

                entity.HasIndex(x => x.Name);
            });

            modelBuilder.Entity<RegistryHost>(entity =>
            {
                entity.ToTable("registry_host");
                entity.HasKey(x => x.Id).HasName("id");
                entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(x => x.ExecuterId).HasColumnName("executer_id");
                entity.Property(x => x.Host).HasColumnName("host").HasMaxLength(100).IsRequired();
                entity.Property(x => x.Order).HasColumnName("order");

                entity.HasOne(x => x.JobExecuter).WithMany(x => x.RegistryHosts).HasForeignKey(x => x.ExecuterId);
            });

            
        }
    }
}
