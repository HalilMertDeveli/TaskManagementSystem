
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
                Defination = request.Definition,
            };
        }

        public static AppTasks ToMap(this AppTaskCreateRequest request)
        {
            return new AppTasks
            {
                Description = request.Description ?? string.Empty, // null kontrolü ekledim
                Title = request.Title ?? string.Empty, // null kontrolü ekledim
                PriorityId = request.PriorityId,
                State = false,
                StartDate = request.StartDate, // ✅ Yeni eklendi
                EndDate = request.EndDate      // ✅ Yeni eklendi
            };
        }



        public static AppTaskListDto ToMap(this AppTasks appTask)
        {
            return new AppTaskListDto(
                appTask.Id,
                appTask.Title,
                appTask.Description,
                appTask?.Priority?.Defination,
                appTask?.State ?? false,
                appTask.AppUserId,
                appTask.AppUserId.HasValue ? appTask.AppUser?.Name + " " + appTask.AppUser?.Surname : null,
                appTask.PriorityId,
                appTask.StartDate ?? DateTime.Now,  // Eğer StartDate null ise, minimum bir tarih verilir.
                appTask.EndDate ?? DateTime.Today   // Eğer EndDate null ise, minimum bir tarih verilir.
            );
        }

        public static List<MemberListDto> ToMap(this List<AppUser> users)
        {
            return users.Select(x => new MemberListDto(x.Id, x.Name, x.Surname, x.UserName)).ToList();
        }

        public static List<TaskReportListDto> ToMap(this List<TaskReport> list)
        {
            return list.Select(x => new TaskReportListDto(x.Id, x.Defination, x.Detail, x.AppTaskId, x.AppTask.Title)).ToList();
        }

    }
}
