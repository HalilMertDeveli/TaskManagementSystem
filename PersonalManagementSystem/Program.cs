using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Persistence;
using HMD.TaskManagement.Persistence.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PersonalManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Every unsafe HTTP method (POST/PUT/PATCH/DELETE) must carry a valid antiforgery token.
            // Forms rendered with the form tag helper include the token automatically.
            builder.Services.AddControllersWithViews(options =>
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

            //IOC
            builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.AddApplicationServices();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opt =>
                {
                    opt.Cookie.Name = "PersonalManagementLCookie";
                    opt.Cookie.HttpOnly = true;
                    opt.Cookie.SameSite = SameSiteMode.Strict;
                    opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                });

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TaskManagementContext>();
                var pendingMigrations = 0;
                try
                {
                    pendingMigrations = db.Database.GetPendingMigrations().Count();
                }
                catch (Exception ex)
                {
                    app.Logger.LogWarning(ex, "Could not read pending migrations (connection or model issue).");
                }

                try
                {
                    db.Database.Migrate();
                }
                catch (Exception ex)
                {
                    // Migrate() can fail while the DB is still usable (history mismatch, permissions, etc.).
                    app.Logger.LogWarning(ex, "EF Core Migrate() failed; unassigned tasks may still fail until schema is fixed.");
                }

                try
                {
                    if (!db.Database.CanConnect())
                    {
                        app.Logger.LogWarning("Database is not reachable; skipped schema repair and role seed.");
                    }
                    else
                    {
                        // Matches EF model: unassigned tasks use NULL AppUserId. Some DBs never applied the nullable migration.
                        db.Database.ExecuteSqlRaw(@"
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.tables t ON c.object_id = t.object_id
    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
    WHERE s.name = N'dbo' AND t.name = N'Tasks' AND c.name = N'AppUserId' AND c.is_nullable = 0)
    ALTER TABLE [dbo].[Tasks] ALTER COLUMN [AppUserId] INT NULL;

IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.tables t ON c.object_id = t.object_id
    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
    WHERE s.name = N'dbo' AND t.name = N'AppTasks' AND c.name = N'AppUserId' AND c.is_nullable = 0)
    ALTER TABLE [dbo].[AppTasks] ALTER COLUMN [AppUserId] INT NULL;
");

                        // Register flow depends on these role ids (Admin=1, Member=2).
                        // Some databases miss seed data, so we ensure required roles exist.
                        db.Database.ExecuteSqlRaw(@"
IF NOT EXISTS (SELECT 1 FROM [Roles] WHERE [Id] IN (1,2))
BEGIN
    SET IDENTITY_INSERT [Roles] ON;
    IF NOT EXISTS (SELECT 1 FROM [Roles] WHERE [Id] = 1)
        INSERT INTO [Roles] ([Id], [Defination]) VALUES (1, 'Admin');
    IF NOT EXISTS (SELECT 1 FROM [Roles] WHERE [Id] = 2)
        INSERT INTO [Roles] ([Id], [Defination]) VALUES (2, 'Member');
    SET IDENTITY_INSERT [Roles] OFF;
END");
                    }

                    if (pendingMigrations > 0)
                    {
                        app.Logger.LogInformation("There were {Count} pending migration(s) before Migrate(); verify __EFMigrationsHistory if errors persist.", pendingMigrations);
                    }
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Database schema repair or role seed failed; check connection string and SQL permissions.");
                }
            }

            // Configure the HTTP request pipeline".
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();//dışarı açmak wwwroot içerisinde olanları
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();





            //halil.com/admin/home/Index
            //hallil.com/home/index //ikisinde de özel olan, önde olan ilk olarak denenmesi gerekemektedir. 

            // Area route must use {area:exists}; otherwise URLs like /Account/Login are parsed as
            // Area=Account, Controller=Login and never reach AccountController.
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");


            app.Run();
        }
    }
}
