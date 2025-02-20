using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMD.TaskManagement.Domain.Entities
{
    public class AppTasks : BaseEntity
    {
        public string Title { get; set; } = null!;

        public string? Description { get; set; }
        public int AppUserId { get; set; }
        public int PriorityId { get; set; }
        //look up table ? 
        public bool State { get; set; }

    }
}
