using HMD.TaskManagement.Domain.Entities;
using HMD.TaskManagement.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HMD.TaskManagement.Persistence.Context
{
    public class TaskManagementContext : DbContext
    {
        public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options)
        {

        }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<AppTasks> AppTasks { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<TaskReport> TaskReports { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AppRoleConfiguration());
            modelBuilder.ApplyConfiguration(new AppTaskConfiguration());
            modelBuilder.ApplyConfiguration(new AppUserConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new PriorityConfiguration());
            modelBuilder.ApplyConfiguration(new TaskReportConfiguration());

            base.OnModelCreating(modelBuilder);
        }


    }
}
