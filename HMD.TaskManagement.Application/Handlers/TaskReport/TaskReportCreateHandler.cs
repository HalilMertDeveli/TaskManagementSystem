using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Validators.TaskReport;
using MediatR;
using HMD.TaskManagement.Application.Extensions;

namespace HMD.TaskManagement.Application.Handlers.TaskReport
{
    public class TaskReportCreateHandler : IRequestHandler<TaskReportCreateRequest, Result<NoData>>
    {
        private readonly ITaskReportRepository repository;
        private readonly INotificationRepository notificationRepository;
        private readonly IAppTaskRepository appTaskRepository;
        private readonly IUserRepository userRepository;

        public TaskReportCreateHandler(ITaskReportRepository repository, INotificationRepository notificationRepository, IAppTaskRepository appTaskRepository, IUserRepository userRepository)
        {
            this.repository = repository;
            this.notificationRepository = notificationRepository;
            this.appTaskRepository = appTaskRepository;
            this.userRepository = userRepository;
        }

        public async Task<Result<NoData>> Handle(TaskReportCreateRequest request, CancellationToken cancellationToken)
        {
            var validator = new TaskReportCreateRequestValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return new Result<NoData>(new NoData(), false, null, validationResult.Errors.ToMap());
            }

            // Resource-level check: reports can only be written for tasks assigned to the signed-in member.
            var task = await this.appTaskRepository.GetByFilterAsNoTrackingAsync(x => x.Id == request.TaskId && x.AppUserId == request.UserId);
            if (task == null)
                return new Result<NoData>(new NoData(), false, "İş emri bulunamadı", null);

            var rowCount = await this.repository.CreateAsync(new Domain.Entities.TaskReport
            {
                AppTaskId = task.Id,
                Defination = request.Definition!,
                Detail = request.Detail!
            });
            if (rowCount > 0)
            {
                await this.notificationRepository.NotifyAdminsAsync(this.userRepository, $"Bir rapor yazıldı, başlık : {request.Definition}");
                return new Result<NoData>(new NoData(), true, null, null);
            }
            return new Result<NoData>(new NoData(), false, "Sistemsel bir hata oluştu, sistem üreticinize başvurun", null);
        }
    }
}
