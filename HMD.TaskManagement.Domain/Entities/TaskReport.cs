using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMD.TaskManagement.Domain.Entities
{
    public class TaskReport
    {
        public int Id { get; set; }
        public string Defination { get; set; } = null!;
        public string  Detail { get; set; }
        public int AppTaskId { get; set; }

    }
}
