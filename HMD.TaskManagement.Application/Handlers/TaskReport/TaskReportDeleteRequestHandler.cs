using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.TaskReport
{
    public class TaskReportDeleteRequestHandler : IRequestHandler<TaskReportDeleteRequest, Result<NoData>>
    {
        private readonly ITaskReportRepository repository;

        public TaskReportDeleteRequestHandler(ITaskReportRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<NoData>> Handle(TaskReportDeleteRequest request, CancellationToken cancellationToken)
        {
            // Resource-level check: the report's task must be assigned to the signed-in member.
            var deletedEntity = await this.repository.GetByFilter(x => x.Id == request.Id && x.AppTask!.AppUserId == request.UserId, false);
            if (deletedEntity != null)
            {
                await this.repository.DeleteAsync(deletedEntity);
                return new Result<NoData>(new NoData(), true, null, null);
            }
            return new Result<NoData>(new NoData(), false, "Silinecek rapor bulunamadı", null);
        }
    }
}
