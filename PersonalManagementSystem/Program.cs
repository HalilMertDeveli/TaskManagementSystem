using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Persistence;

namespace PersonalManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //IOC
            builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.AddApplicationServices();

            var app = builder.Build();

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

            app.UseAuthorization();

            //halil.com/admin/home/Index
            //hallil.com/home/index //ikisinde de özel olan, önde olan ilk olarak denenmesi gerekemektedir. 

            //özel olan önde olacak 
            app.MapControllerRoute(
                name: "default",
                pattern: "{Area}/{Controller=Home}/{Action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");


            app.Run();
        }
    }
}
