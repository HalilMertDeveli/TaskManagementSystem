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
    public class TaskReportRepository:ITaskReportRepository
    {
        private readonly TaskManagementContext context;

        public TaskReportRepository(TaskManagementContext context)
        {
            this.context = context;
        }

        

        public async Task<List<TaskReport>> GetAllByFilterAsync(Expression<Func<TaskReport, bool>> filter, bool asNoTracking = true)
        {

            if (asNoTracking)
            {
                return await this.context.TaskReports.AsNoTracking().Where(filter).ToListAsync();
            }
            return await this.context.TaskReports.Where(filter).ToListAsync();
        }
    }
}

