using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Domain.Entities;

namespace HMD.TaskManagement.Application.Interfaces
{
    public interface IAppTaskRepository
    {
        Task<List<AppTasks>> GetAllAsync();
    }
}
