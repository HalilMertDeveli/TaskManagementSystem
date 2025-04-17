using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.Notification
{
    public class NotificationListByUserIdHandler : IRequestHandler<NotificationListByUserIdRequest, Result<List<NotificationListDto>>>
    {

        private readonly INotificationRepository repository;

        public NotificationListByUserIdHandler(INotificationRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<List<NotificationListDto>>> Handle(NotificationListByUserIdRequest request, CancellationToken cancellationToken)
        {
            var result = await this.repository.GetAllByFilterAsync(x => x.AppUserID == request.UserId);


            var mappedData = result.Select(x => new NotificationListDto(x.Id, x.Description, x.State, x.AppUserID)).ToList();
            return new Result<List<NotificationListDto>>(mappedData, true, null, null);
        }
    }
}
