namespace HMD.TaskManagement.Domain.Entities
{
    public class Priority : BaseEntity
    {
        public string Defination { get; set; } = null!;

        #region NavigationProperty

        public List<AppTasks>? Tasks { get; set; }

        #endregion

    }
}
