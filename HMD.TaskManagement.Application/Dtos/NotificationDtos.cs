﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMD.TaskManagement.Application.Dtos
{
    public record NotificationListDto(int Id, string Description, bool State, int? AppUserId);

}
