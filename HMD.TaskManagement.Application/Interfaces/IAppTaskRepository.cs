using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Domain.Entities;
using System.Linq.Expressions;

namespace HMD.TaskManagement.Application.Interfaces
{
    public interface IAppTaskRepository
    {
        Task<PagedData<AppTasks>> GetAllAsync(int activePage, string? s = null, int pageSize = 10);
        Task<int> CreateAsync(AppTasks appTasks);

        Task DeleteAsync(AppTasks deleted);

        Task<AppTasks?> GetByFilterAsync(Expression<Func<AppTasks, bool>> filter);

        Task<AppTasks?> GetByFilterAsNoTracking(Expression<Func<AppTasks, bool>> filter);
        Task<int> SaveChangesAsync();
    }
}
