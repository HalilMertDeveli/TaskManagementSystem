using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMD.TaskManagement.Application.Dtos
{
    public record DashboardDto(int TaskCount, int UserCount, int NotificationCount);

}
