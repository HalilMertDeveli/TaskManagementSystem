using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var updated = await this.repository.GetByFilterAsync(x => x.Id == request.Id);
            if (updated == null)
                return new Result<NoData>(new NoData(), false, "Bildirim bulunamadı", null);

            updated.State = true;

            await this.repository.SaveChangesAsync();

            return new Result<NoData>(new NoData(), true, null, null);
        }
    }
}
