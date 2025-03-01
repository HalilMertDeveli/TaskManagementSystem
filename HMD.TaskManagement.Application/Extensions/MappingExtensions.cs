
using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Domain.Entities;

namespace HMD.TaskManagement.Application.Extensions
{
    public static class MappingExtensions
    {
        public static AppUser ToMap(this RegisterRequest request)
        {
            return new AppUser
            {
                AppRoleId = (int)RoleType.Member,
                Name = request.Name,
                Password = request.Password,
                Surname = request.Surname,
                UserName = request.Username,

            };
        }
    }
}
