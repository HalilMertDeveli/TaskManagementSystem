using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Domain.Entities;

namespace HMD.TaskManagement.Application.Extensions
{
    public static class NotificationExtensions
    {
        /// <summary>
        /// Sends the notification to every user with the Admin role (replaces the former hard-coded AppUserID = 1).
        /// </summary>
        public static async Task NotifyAdminsAsync(this INotificationRepository notifications, IUserRepository users, string description)
        {
            var admins = await users.GetAllByFilterAsync(x => x.AppRoleId == (int)RoleType.Admin);
            foreach (var admin in admins)
            {
                await notifications.SendNotification(new Notification
                {
                    AppUserID = admin.Id,
                    Description = description,
                    State = false,
                });
            }
        }
    }
}
