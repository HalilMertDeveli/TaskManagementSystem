using HMD.TaskManagement.Domain.Entities;
using HMD.TaskManagement.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMD.TaskManagement.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {

        private readonly TaskManagementContext context;

        public NotificationRepository(TaskManagementContext context)
        {
            this.context = context;
        }

        public async Task<List<Notification>?> GetAllByFilterAsync(Expression<Func<Notification, bool>> filter, bool asNoTracking = true)
        {
            if (asNoTracking)
            {
                return await this.context.Notifications.AsNoTracking().Where(filter).OrderByDescending(x => x.Id).ToListAsync();
            }
            return await this.context.Notifications.Where(filter).OrderByDescending(x => x.Id).ToListAsync();
        }

        public async Task<Notification?> GetByFilterAsNoTrackingAsync(Expression<Func<Notification, bool>> filter)
        {
            return await this.context.Notifications.AsNoTracking().SingleOrDefaultAsync(filter);
        }

        public async Task<Notification?> GetByFilterAsync(Expression<Func<Notification, bool>> filter)
        {
            return await this.context.Notifications.SingleOrDefaultAsync(filter);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this.context.SaveChangesAsync();
        }

        public async Task<int> SendNotification(Notification notification)
        {
            await this.context.AddAsync(notification);
            return await this.context.SaveChangesAsync();
        }
    }
}
