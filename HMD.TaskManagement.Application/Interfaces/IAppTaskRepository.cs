using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Domain.Entities;

namespace HMD.TaskManagement.Application.Interfaces
{
    public interface IAppTaskRepository
    {
        Task<PagedData<AppTasks>> GetAllAsync(int activePage, int pageSize=10);
    }
}
