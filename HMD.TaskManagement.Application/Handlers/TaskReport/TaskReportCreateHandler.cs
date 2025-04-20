using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Validators.TaskReport;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Extensions;

namespace HMD.TaskManagement.Application.Handlers.TaskReport
{
    public class TaskReportCreateHandler : IRequestHandler<TaskReportCreateRequest, Result<NoData>>
    {
        private readonly ITaskReportRepository repository;
        private readonly INotificationRepository notificationRepository;

        public TaskReportCreateHandler(ITaskReportRepository repository, INotificationRepository notificationRepository)
        {
            this.repository = repository;
            this.notificationRepository = notificationRepository;
        }

        public async Task<Result<NoData>> Handle(TaskReportCreateRequest request, CancellationToken cancellationToken)
        {
            var validator = new TaskReportCreateRequestValidator();
            var validationResult = validator.Validate(request);


            if (validationResult.IsValid)
            {
                var rowCount = await this.repository.CreateAsync(new Domain.Entities.TaskReport
                {
                    AppTaskId = request.TaskId,
                    Defination = request.Definition,
                    Detail = request.Detail
                });
                if (rowCount > 0)
                {
                    await this.notificationRepository.SendNotification(new Domain.Entities.Notification
                    {
                        AppUserID = 1,//admin değişecek sadece 1 tane admin varmış gibi atama yapılıyor
                        Description = $"Bir rapor yazıldı, başlık : {request.Definition}",
                        State = false,

                    });
                    return new Result<NoData>(new NoData(), true, null, null);
                }
                return new Result<NoData>(new NoData(), false, "Sistemsel bir hata oluştu, sistem üreticinize başvurun", null);
            }
            else
            {
                var errors = validationResult.Errors.ToMap();
                return new Result<NoData>(new NoData(), false, null, errors);
            }
        }
    }
}
