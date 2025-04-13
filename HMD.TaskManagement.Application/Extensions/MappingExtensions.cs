
using HMD.TaskManagement.Application.Dtos;
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

        public static Priority ToMap(this PriorityCreateRequest request)
        {
            return new Priority
            {
                Defination = request.Definition

            };
        }

        public static AppTasks ToMap(this AppTaskCreateRequest request)
        {
            return new AppTasks()
            {
                Description = request.Description,
                PriorityId = request.PriorityId,
                Title = request.Title,
                State = false,
            };
        }

        public static AppTaskListDto ToMap(this AppTasks appTask)
        {
            return new AppTaskListDto(appTask.Id, appTask.Title, appTask.Description, appTask.Priority.Defination,
                appTask.State, appTask.AppUserId,
                appTask.AppUserId.HasValue ? appTask.AppUser?.Name + " " + appTask.AppUser?.Surname : null,
                appTask.PriorityId);

        }

    }
}
