using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMD.TaskManagement.Persistence.Context
{
    public class TaskManagementContext:DbContext
    {
        public TaskManagementContext(DbContextOptions<TaskManagementContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<AppTasks> AppTasks { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<TaskReport> TaskReports { get; set; }

    }
}
