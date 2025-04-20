using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Domain.Entities;
using System.Linq.Expressions;

namespace HMD.TaskManagement.Application.Interfaces
{
    public interface IAppTaskRepository
    {

        Task<PagedData<AppTasks>> GetAllAsync(int activePage, string? s = null, int pageSize = 10);

        Task<int> CreateAsync(AppTasks task);
        Task DeleteAsync(AppTasks deleted);
        Task<AppTasks?> GetByFilterAsync(Expression<Func<AppTasks, bool>> filter);
        Task<AppTasks?> GetByFilterAsNoTrackingAsync(Expression<Func<AppTasks, bool>> filter);

        Task<List<AppTasks>?> GetAllByFilter(Expression<Func<AppTasks, bool>> filter);

        Task<PagedData<AppTasks>> GetAllByUserIdAsync(int activePage, int userId, string? s = null, int pageSize = 10);

        Task<int> SaveChangesAsync();
    }
}

