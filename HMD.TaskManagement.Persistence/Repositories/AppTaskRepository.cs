using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Domain.Entities;
using HMD.TaskManagement.Persistence.Context;
using HMD.TaskManagement.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace HMD.TaskManagement.Persistence.Repositories
{
    public class AppTaskRepository : IAppTaskRepository
    {
        private readonly TaskManagementContext context;

        public AppTaskRepository(TaskManagementContext context)
        {
            this.context = context;
        }

        public async Task<PagedData<AppTasks>> GetAllAsync(int activePage, int pageSize = 10)
        {
            var list = await this.context.Tasks.Include(x => x.Priority).AsNoTracking()
                .ToPagedAsync(activePage, pageSize);
            return list;
        }


    }
}
