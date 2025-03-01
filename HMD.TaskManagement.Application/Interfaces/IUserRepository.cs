using HMD.TaskManagement.Domain.Entities;
using System.Linq.Expressions;

namespace HMD.TaskManagement.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByFilterAsync(Expression<Func<AppUser, bool>> filter, bool asNoTracking = true);

        Task<int> CreateUserAsync(AppUser user);
    }
}
