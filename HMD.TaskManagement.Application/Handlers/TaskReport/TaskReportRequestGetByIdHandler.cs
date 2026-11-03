using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.TaskReport
{
    public class TaskReportRequestGetByIdHandler : IRequestHandler<TaskReportGetByIdRequest, Result<TaskReportListDto?>>
    {
        private readonly ITaskReportRepository repository;

        public TaskReportRequestGetByIdHandler(ITaskReportRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<TaskReportListDto?>> Handle(TaskReportGetByIdRequest request, CancellationToken cancellationToken)
        {
            // Resource-level check: the report's task must be assigned to the signed-in member.
            var result = await this.repository.GetByFilter(x => x.Id == request.Id && x.AppTask!.AppUserId == request.UserId);
            if (result == null)
                return new Result<TaskReportListDto?>(null, false, "Rapor bulunamadı", null);

            return new Result<TaskReportListDto?>(new TaskReportListDto(result.Id, result.Defination, result.Detail, result.AppTaskId, result.AppTask?.Title), true, null, null);
        }
    }
}
