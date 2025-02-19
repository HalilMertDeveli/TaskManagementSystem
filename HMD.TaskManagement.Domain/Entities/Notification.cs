using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMD.TaskManagement.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public bool  State { get; set; }
        public int AppUserID { get; set; }
    }
}
