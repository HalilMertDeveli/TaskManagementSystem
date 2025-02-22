using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Domain.Entities;
using HMD.TaskManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HMD.TaskManagement.Persistence.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly TaskManagementContext context;

        public UserRepository(TaskManagementContext context)
        {
            this.context = context;
        }

        public async Task<AppUser?> GetByFilter(Expression<Func<AppUser,bool>> filter,bool asNoTracking=true)
        {
            if (asNoTracking)
            {
                return await this.context.Users.AsNoTracking().SingleOrDefaultAsync(filter);
            }

            return await this.context.Users.SingleOrDefaultAsync(filter); 
        }
    }
}
