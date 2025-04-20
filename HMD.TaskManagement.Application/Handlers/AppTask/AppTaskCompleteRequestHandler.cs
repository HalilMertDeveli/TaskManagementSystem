using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMD.TaskManagement.Application.Handlers.AppTask
{
    public class AppTaskCompleteRequestHandler : IRequestHandler<AppTaskCompleteRequest, Result<NoData>>
    {
        private readonly IAppTaskRepository appTaskRepository;
        private readonly INotificationRepository notificationRepository;

        public AppTaskCompleteRequestHandler(IAppTaskRepository appTaskRepository, INotificationRepository notificationRepository)
        {
            this.appTaskRepository = appTaskRepository;
            this.notificationRepository = notificationRepository;
        }

        public async Task<Result<NoData>> Handle(AppTaskCompleteRequest request, CancellationToken cancellationToken)
        {

            var updated = await this.appTaskRepository.GetByFilterAsync(x => x.Id == request.Id);

            updated.State = true;

            await this.appTaskRepository.SaveChangesAsync();

            await this.notificationRepository.SendNotification(new Domain.Entities.Notification
            {
                State = false,
                AppUserID = 1,//burada yine statik bir şekilde bildirim yollama işlemi var , 
                Description = $"{updated.Title} adlı iş emri tamamlandı",

            });

            return new Result<NoData>(new NoData(), true, null, null);

        }
    }
}
