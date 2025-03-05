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
            services.AddDbContext<TaskManagementContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("Local"));
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPriorityRepository, PriorityRepository>();
            services.AddScoped<IAppTaskRepository, AppTaskRepository>();
        }

    }
}
