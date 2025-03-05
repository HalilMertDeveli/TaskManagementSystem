using System.Linq.Expressions;
using HMD.TaskManagement.Domain.Entities;

namespace HMD.TaskManagement.Application.Interfaces
{
    public interface IPriorityRepository
    {
        Task<List<Priority>> GetAllAsync();
        Task<int> CreateAsync(Priority priority);

        Task<Priority?> GetByFilterAsNoTrackingAsync(Expression<Func<Priority, bool>> filter);

        Task<Priority?> GetByFilterAsync(Expression<Func<Priority, bool>> filter);
        Task DeleteAsync(Priority priority);

        Task<int> SaveChangesAsync();
    }
}
