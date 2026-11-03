using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Domain.Entities;
using HMD.TaskManagement.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMD.TaskManagement.Tests.Infrastructure
{
    /// <summary>
    /// Boots the real application (Program.cs, MVC filters, cookie auth, EF Core migrations) against a
    /// throw-away SQL Server database. The server comes from the TMS_TEST_SQLSERVER environment variable
    /// (CI uses a SQL Server container) and defaults to SQL Server LocalDB for local runs.
    /// </summary>
    public sealed class TmsWebApplicationFactory : WebApplicationFactory<PersonalManagementSystem.Program>, IAsyncLifetime
    {
        public const string DefaultServer = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=True;TrustServerCertificate=True";

        private readonly string connectionString;

        public TmsWebApplicationFactory()
        {
            var server = Environment.GetEnvironmentVariable("TMS_TEST_SQLSERVER");
            var builder = new SqlConnectionStringBuilder(string.IsNullOrWhiteSpace(server) ? DefaultServer : server)
            {
                InitialCatalog = $"TmsTests_{Guid.NewGuid():N}",
            };
            connectionString = builder.ConnectionString;
        }

        public IPasswordHasher<AppUser> Hasher { get; } = new PasswordHasher<AppUser>();

        public int PriorityId { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Local"] = connectionString,
                });
            });
        }

        public async Task InitializeAsync()
        {
            // Accessing Services starts the host; Program.cs applies the EF Core migrations and seeds the roles.
            await using var scope = Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<TaskManagementContext>();

            if ((await db.Database.GetPendingMigrationsAsync()).Any())
                throw new InvalidOperationException("Migrations were not applied at startup.");

            var priority = new Priority { Defination = "Normal" };
            db.Priorities.Add(priority);
            await db.SaveChangesAsync();
            PriorityId = priority.Id;
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            try
            {
                await using var scope = Services.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<TaskManagementContext>();
                await db.Database.EnsureDeletedAsync();
            }
            finally
            {
                await base.DisposeAsync();
            }
        }

        public HttpClient CreateBrowserClient() => CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true,
        });

        public async Task<T> WithDbAsync<T>(Func<TaskManagementContext, Task<T>> action)
        {
            await using var scope = Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<TaskManagementContext>();
            return await action(db);
        }

        public Task WithDbAsync(Func<TaskManagementContext, Task> action)
            => WithDbAsync(async db => { await action(db); return true; });

        /// <summary>Creates a user. With <paramref name="legacyPlaintext"/> the password is stored un-hashed, as before this change.</summary>
        public Task<TestUser> CreateUserAsync(RoleType role, string? password = null, bool legacyPlaintext = false)
        {
            var suffix = Guid.NewGuid().ToString("N")[..12];
            password ??= "Pw-" + Guid.NewGuid().ToString("N");
            var user = new AppUser
            {
                UserName = $"{role.ToString().ToLowerInvariant()}_{suffix}",
                Name = "Test",
                Surname = $"User_{suffix}",
                AppRoleId = (int)role,
            };
            user.Password = legacyPlaintext ? password : Hasher.HashPassword(user, password);

            return WithDbAsync(async db =>
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return new TestUser(user.Id, user.UserName, password, role);
            });
        }

        public Task<int> CreateTaskAsync(int? assigneeId, string? title = null) => WithDbAsync(async db =>
        {
            var task = new AppTasks
            {
                Title = title ?? $"Task {Guid.NewGuid():N}",
                Description = "Integration test task",
                PriorityId = PriorityId,
                AppUserId = assigneeId,
                State = false,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(7),
            };
            db.Tasks.Add(task);
            await db.SaveChangesAsync();
            return task.Id;
        });

        public Task<int> CreateReportAsync(int taskId) => WithDbAsync(async db =>
        {
            var report = new TaskReport { AppTaskId = taskId, Defination = "Original title", Detail = "Original detail" };
            db.TaskReports.Add(report);
            await db.SaveChangesAsync();
            return report.Id;
        });

        public Task<int> CreateNotificationAsync(int userId) => WithDbAsync(async db =>
        {
            var notification = new Notification { AppUserID = userId, Description = "Test notification", State = false };
            db.Notifications.Add(notification);
            await db.SaveChangesAsync();
            return notification.Id;
        });

        public Task<AppUser?> FindUserAsync(int id) => WithDbAsync(db => db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id));
        public Task<AppTasks?> FindTaskAsync(int id) => WithDbAsync(db => db.Tasks.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id));
        public Task<TaskReport?> FindReportAsync(int id) => WithDbAsync(db => db.TaskReports.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id));
        public Task<Priority?> FindPriorityAsync(int id) => WithDbAsync(db => db.Priorities.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id));
        public Task<Notification?> FindNotificationAsync(int id) => WithDbAsync(db => db.Notifications.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id));
    }

    public sealed record TestUser(int Id, string UserName, string Password, RoleType Role);

    [CollectionDefinition(Name)]
    public sealed class IntegrationCollection : ICollectionFixture<TmsWebApplicationFactory>
    {
        public const string Name = "Integration";
    }
}
