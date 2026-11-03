using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.Notification
{
    public class NotificationUpdateHandler : IRequestHandler<NotificationUpdateRequest, Result<NoData>>
    {
        private readonly INotificationRepository repository;

        public NotificationUpdateHandler(INotificationRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<NoData>> Handle(NotificationUpdateRequest request, CancellationToken cancellationToken)
        {
            // Resource-level check: users can only mark their own notifications as read.
            var updated = await this.repository.GetByFilterAsync(x => x.Id == request.Id && x.AppUserID == request.UserId);
            if (updated == null)
                return new Result<NoData>(new NoData(), false, "Bildirim bulunamadı", null);

            updated.State = true;

            await this.repository.SaveChangesAsync();

            return new Result<NoData>(new NoData(), true, null, null);
        }
    }
}
