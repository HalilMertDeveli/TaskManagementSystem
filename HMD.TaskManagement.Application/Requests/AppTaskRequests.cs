﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using MediatR;

namespace HMD.TaskManagement.Application.Requests
{
    public record AppTaskListRequest():IRequest<Result<List<AppTaskListDto>>>;

}
