﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMD.TaskManagement.Domain.Entities
{
    public class AppRole : BaseEntity
    {
        public string Defination { get; set; } = null!; //null olamaz

        #region NavigationProperties

        public List<AppUser>? Users { get; set; }


        #endregion

    }
}
