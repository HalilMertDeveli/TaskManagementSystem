using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMD.TaskManagement.Application.Dtos
{
    public record AppTaskListDto(int Id, string Title, string Description, string? PriorityDefinition, bool State);
}
