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
    }
}
