using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Domain.Entities;
using HMD.TaskManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HMD.TaskManagement.Persistence.Repositories
{
    public class AppTaskRepository:IAppTaskRepository
    {
        private readonly TaskManagementContext context;

        public AppTaskRepository(TaskManagementContext context)
        {
            this.context = context;
        }

        public async Task<List<AppTasks>> GetAllAsync()
        {
            return await this.context.Tasks.Include(x=>x.Priority).AsNoTracking().ToListAsync();
            
        }
    }
}
