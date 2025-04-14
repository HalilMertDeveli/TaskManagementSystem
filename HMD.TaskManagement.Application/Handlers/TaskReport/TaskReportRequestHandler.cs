using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.Account
{
    public class TaskReportRequestHandler:IRequestHandler<TaskReportGetByTaskIdRequest,Result<List<TaskReportListDto>>>
    {
        private readonly ITaskReportRepository repository;

        public TaskReportRequestHandler(ITaskReportRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<List<TaskReportListDto>>> Handle(TaskReportGetByTaskIdRequest request, CancellationToken cancellationToken)
        {
           var result = await this.repository.GetAllByFilterAsync(x => x.AppTaskId == request.Id, false);

           return new Result<List<TaskReportListDto>>(result.ToMap(),true,null,null);
        }
    }
}
