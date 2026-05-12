using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Persistence.Context;
using HMD.TaskManagement.Persistence.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace HMD.TaskManagement.Persistence
{
    public static class IOCExtensions
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Local");
            string? dataSource = null;
            string? initialCatalog = null;
            try
            {
                var csb = new SqlConnectionStringBuilder(connectionString);
                dataSource = csb.DataSource;
                initialCatalog = csb.InitialCatalog;
            }
            catch
            {
                // Ignore parse failures in debug instrumentation.
            }

            // #region agent log
            try { File.AppendAllText("C:/Users/clt/Desktop/TaskManagementSystem/debug-8b40ae.log", JsonSerializer.Serialize(new { sessionId = "8b40ae", runId = "investigation-1", hypothesisId = "H1", location = "IOCExtensions.cs:AddPersistenceServices", message = "Resolved Local connection string", data = new { hasConnectionString = !string.IsNullOrWhiteSpace(connectionString), dataSource, initialCatalog }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + Environment.NewLine); } catch { }
            // #endregion

            services.AddDbContext<TaskManagementContext>(opt =>
            {
                opt.UseSqlServer(connectionString);
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPriorityRepository, PriorityRepository>();
            services.AddScoped<IAppTaskRepository, AppTaskRepository>();
            services.AddScoped<ITaskReportRepository, TaskReportRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
        }

    }
}
