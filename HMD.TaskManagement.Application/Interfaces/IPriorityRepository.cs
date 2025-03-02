using HMD.TaskManagement.Domain.Entities;

namespace HMD.TaskManagement.Application.Interfaces
{
    public interface IPriorityRepository
    {
        Task<List<Priority>> GetAllAsync();
        Task<int> CreateAsync(Priority priority);
    }
}
