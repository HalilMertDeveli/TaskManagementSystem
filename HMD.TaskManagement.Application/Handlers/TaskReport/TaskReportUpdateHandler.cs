using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Validators.TaskReport;

namespace HMD.TaskManagement.Application.Handlers.TaskReport
{
    public class TaskReportUpdateHandler : IRequestHandler<TaskReportUpdateRequest, Result<NoData>>
    {
        private readonly ITaskReportRepository repository;

        public TaskReportUpdateHandler(ITaskReportRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<NoData>> Handle(TaskReportUpdateRequest request, CancellationToken cancellationToken)
        {
            var validator = new TaskReportUpdateRequestValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return new Result<NoData>(new NoData(), false, null, validationResult.Errors.ToMap());
            }

            // Resource-level check: the report's task must be assigned to the signed-in member.
            var updatedEnity = await this.repository.GetByFilter(x => x.Id == request.Id && x.AppTask!.AppUserId == request.UserId, false);
            if (updatedEnity == null)
                return new Result<NoData>(new NoData(), false, "Rapor bulunamadı", null);

            updatedEnity.Defination = request.Definition!;
            updatedEnity.Detail = request.Detail!;

            await this.repository.SaveChangesAsync();
            return new Result<NoData>(new NoData(), true, null, null);
        }
    }
}
