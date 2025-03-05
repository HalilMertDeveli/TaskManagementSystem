using System.Linq.Expressions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Domain.Entities;
using HMD.TaskManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HMD.TaskManagement.Persistence.Repositories
{
    public class PriorityRepository : IPriorityRepository
    {
        private readonly TaskManagementContext context;

        public PriorityRepository(TaskManagementContext context)
        {
            this.context = context;
        }

        public Task<List<Priority>> GetAllAsync()
        {
            return this.context.Priorities.AsNoTracking().ToListAsync();
        }

        public async Task<int> CreateAsync(Priority priority)
        {
            await this.context.Priorities.AddAsync(priority);
            return await this.context.SaveChangesAsync();
        }

        public async Task<Priority?> GetByFilterAsNoTrackingAsync(Expression<Func<Priority, bool>> filter)
        {
            return await this.context.Priorities.AsNoTracking().SingleOrDefaultAsync(filter);
        }

        public async Task<Priority?> GetByFilterAsync(Expression<Func<Priority, bool>> filter)
        {
            return await this.context.Priorities.SingleOrDefaultAsync(filter);
        }

        public async Task DeleteAsync(Priority priority)
        {
            this.context.Priorities.Remove(priority);
            await this.context.SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this.context.SaveChangesAsync();
        }
    }
}
