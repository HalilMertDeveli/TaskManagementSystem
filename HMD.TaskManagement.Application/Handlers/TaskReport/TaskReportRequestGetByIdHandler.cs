using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMD.TaskManagement.Application.Handlers.TaskReport
{
    public class TaskReportRequestGetByIdHandler : IRequestHandler<TaskReportGetByIdRequest, Result<TaskReportListDto>>
    {
        private readonly ITaskReportRepository repository;

        public TaskReportRequestGetByIdHandler(ITaskReportRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<TaskReportListDto>> Handle(TaskReportGetByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await this.repository.GetByFilter(x => x.Id == request.Id, false);
            return new Result<TaskReportListDto>(new TaskReportListDto(result.Id, result.Defination, result.Detail, result.AppTaskId, result.AppTask?.Title), true, null, null);
        }
    }
}
