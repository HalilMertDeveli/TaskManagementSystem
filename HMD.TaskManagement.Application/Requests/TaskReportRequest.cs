using HMD.TaskManagement.Application.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMD.TaskManagement.Application.Requests
{

    public record TaskReportGetByTaskIdRequest(int Id) : IRequest<Result<List<TaskReportListDto>>>;

}
