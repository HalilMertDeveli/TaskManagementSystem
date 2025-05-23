﻿using System.Linq;
using System.Linq.Expressions;
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

        public async Task<int> CreateAsync(AppTasks task)
        {
            await this.context.Tasks.AddAsync(task);
            return await this.context.SaveChangesAsync();
        }

        public async Task<PagedData<AppTasks>> GetAllAsync(int activePage, string? s = null, int pageSize = 10)
        {
            var query = this.context.Tasks.AsQueryable();
            if (!string.IsNullOrEmpty(s))
            {
                query = query.Where(x => x.Title.ToLower().Contains(s.ToLower()));
            }

            var list = await query.Include(x => x.AppUser).Include(x => x.Priority).AsNoTracking().ToPagedAsync(activePage, pageSize);
            return list;
        }

        public async Task<PagedData<AppTasks>> GetAllByUserIdAsync(int activePage, int userId, string? s = null, int pageSize = 10)
        {
            var query = this.context.Tasks.AsQueryable();
            if (!string.IsNullOrEmpty(s))
            {
                query = query.Where(x => x.Title.ToLower().Contains(s.ToLower()));
            }

            var list = await query.Where(x => x.AppUserId == userId).Include(x => x.AppUser).Include(x => x.Priority).AsNoTracking().ToPagedAsync(activePage, pageSize);
            return list;
        }

        public async Task DeleteAsync(AppTasks deleted)
        {
            this.context.Tasks.Remove(deleted);
            await this.context.SaveChangesAsync();
        }
        public async Task<List<AppTasks>?> GetAllByFilter(Expression<Func<AppTasks, bool>> filter)
        {
            return await this.context.Tasks.Where(filter).ToListAsync();
        }

        public async Task<AppTasks?> GetByFilterAsync(Expression<Func<AppTasks, bool>> filter)
        {
            return await this.context.Tasks.SingleOrDefaultAsync(filter);
        }

        public async Task<AppTasks?> GetByFilterAsNoTrackingAsync(Expression<Func<AppTasks, bool>> filter)
        {
            return await this.context.Tasks.AsNoTracking().SingleOrDefaultAsync(filter);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this.context.SaveChangesAsync();
        }

    }
}
