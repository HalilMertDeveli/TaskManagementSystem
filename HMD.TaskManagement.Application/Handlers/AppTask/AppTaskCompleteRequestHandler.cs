using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.AppTask
{
    public class AppTaskCompleteRequestHandler : IRequestHandler<AppTaskCompleteRequest, Result<NoData>>
    {
        private readonly IAppTaskRepository appTaskRepository;
        private readonly INotificationRepository notificationRepository;
        private readonly IUserRepository userRepository;

        public AppTaskCompleteRequestHandler(IAppTaskRepository appTaskRepository, INotificationRepository notificationRepository, IUserRepository userRepository)
        {
            this.appTaskRepository = appTaskRepository;
            this.notificationRepository = notificationRepository;
            this.userRepository = userRepository;
        }

        public async Task<Result<NoData>> Handle(AppTaskCompleteRequest request, CancellationToken cancellationToken)
        {
            // Resource-level check: only the member the task is assigned to may complete it.
            var updated = await this.appTaskRepository.GetByFilterAsync(x => x.Id == request.Id && x.AppUserId == request.UserId);
            if (updated == null)
                return new Result<NoData>(new NoData(), false, "İş emri bulunamadı", null);

            if (updated.State)
                return new Result<NoData>(new NoData(), true, null, null);

            updated.State = true;

            await this.appTaskRepository.SaveChangesAsync();

            await this.notificationRepository.NotifyAdminsAsync(this.userRepository, $"{updated.Title} adlı iş emri tamamlandı");

            return new Result<NoData>(new NoData(), true, null, null);
        }
    }
}
