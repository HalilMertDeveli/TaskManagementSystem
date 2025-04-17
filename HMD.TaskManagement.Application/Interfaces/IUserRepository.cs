using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Domain.Entities;
using System.Linq.Expressions;

namespace HMD.TaskManagement.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByFilterAsync(Expression<Func<AppUser, bool>> filter, bool asNoTracking = true);

        Task<int> CreateUserAsync(AppUser user);

        Task<List<AppUser>> GetAllByFilterAsync(Expression<Func<AppUser, bool>> filter, bool asNoTracking = true);

        Task<PagedData<AppUser>> GetAllAsync(int activePage, string? s = null, int pageSize = 10);

        Task<int> SaveChangesAsync();
        Task DeleteAsync(AppUser deleted);
    }
}
