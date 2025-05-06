namespace HMD.TaskManagement.Domain.Entities
{
    public class AppTasks : BaseEntity
    {
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;
        public int? AppUserId { get; set; }
        public int PriorityId { get; set; }

        //look up table ? 
        public bool State { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime ?EndDate { get; set; }


        #region NavigationProperties
        public AppUser? AppUser { get; set; }

        public Priority Priority { get; set; }

        public List<TaskReport> TaskReports { get; set; }
        #endregion

    }
}
