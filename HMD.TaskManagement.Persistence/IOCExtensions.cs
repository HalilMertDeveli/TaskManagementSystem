using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Persistence.Context;
using HMD.TaskManagement.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMD.TaskManagement.Persistence
{
    public static class IOCExtensions
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // The connection string is resolved from the final configuration when the context is created,
            // so environment variables / test hosts can override ConnectionStrings:Local.
            services.AddDbContext<TaskManagementContext>((serviceProvider, opt) =>
            {
                var config = serviceProvider.GetService<IConfiguration>() ?? configuration;
                opt.UseSqlServer(config.GetConnectionString("Local"));
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPriorityRepository, PriorityRepository>();
            services.AddScoped<IAppTaskRepository, AppTaskRepository>();
            services.AddScoped<ITaskReportRepository, TaskReportRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
        }

    }
}
