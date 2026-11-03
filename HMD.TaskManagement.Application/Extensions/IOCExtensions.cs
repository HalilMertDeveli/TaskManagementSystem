using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HMD.TaskManagement.Application.Extensions
{
    public static class IOCExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {

            services.AddMediatR(configuration =>
                configuration.RegisterServicesFromAssembly(typeof(LoginRequest).Assembly));

            services.AddSingleton<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
        }
    }
}
