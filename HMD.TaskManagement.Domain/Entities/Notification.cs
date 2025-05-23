﻿namespace HMD.TaskManagement.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string Description { get; set; } = null!;
        public bool State { get; set; }
        public int AppUserID { get; set; }

        #region NavigationProperty

        public AppUser? AppUser { get; set; }

        #endregion
    }
}
