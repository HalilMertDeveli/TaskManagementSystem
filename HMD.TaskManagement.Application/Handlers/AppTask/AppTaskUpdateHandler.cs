using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Validators;
using HMD.TaskManagement.Application.Validators.AppTask;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.AppTask
{
    public class AppTaskUpdateHandler : IRequestHandler<AppTaskUpdateRequest, Result<AppTaskDto>>
    {
        private readonly IAppTaskRepository repository;
        private readonly IPriorityRepository priorityRepository;
        private readonly IUserRepository userRepository;
        private readonly INotificationRepository notificationRepository;

        public AppTaskUpdateHandler(IAppTaskRepository repository, IPriorityRepository priorityRepository, IUserRepository userRepository, INotificationRepository notificationRepository)
        {
            this.repository = repository;
            this.priorityRepository = priorityRepository;
            this.userRepository = userRepository;
            this.notificationRepository = notificationRepository;
        }

        public async Task<Result<AppTaskDto>> Handle(AppTaskUpdateRequest request, CancellationToken cancellationToken)
        {

            var priorities = await this.priorityRepository.GetAllAsync();

            var priorityDtoList = priorities.Select(x => new PriorityListDto(x.Id, x.Defination)).ToList();

            var members = await this.userRepository.GetAllByFilterAsync(x => x.AppRoleId == (int)RoleType.Member);

            var memberDtoList = members.Select(x => new MemberListDto(x.Id, x.Name, x.Surname, x.UserName)).ToList();

            var validator = new AppTaskUpdateRequestValidator();
            var validationResult = validator.Validate(request);


            if (validationResult.IsValid)
            {
                var updated = await this.repository.GetByFilterAsync(x => x.Id == request.Id);
                if (updated == null)
                {

                    return new Result<AppTaskDto>(new AppTaskDto(priorityDtoList, memberDtoList), false, "Task bulunamadı", null);
                }

                if (request.AppUserId == null || request.AppUserId == 0)
                {

                    updated.AppUserId = null;
                }
                else
                {

                    if (updated.AppUserId != request.AppUserId)
                    {
                        await this.notificationRepository.SendNotification(new Domain.Entities.Notification
                        {
                            AppUserID = request.AppUserId ?? 0,
                            Description = $"{request.Title} adlı iş üzerinize atandı",
                            State = false,
                        });
                    }

                    updated.AppUserId = request.AppUserId;
                    // send notification


                }

                updated.Title = request.Title;
                updated.Description = request.Description;
                updated.PriorityId = request.PriorityId;

                var rows = await this.repository.SaveChangesAsync();


                if (rows > 0)
                    return new Result<AppTaskDto>(new AppTaskDto(priorityDtoList, memberDtoList), true, null, null);

                return new Result<AppTaskDto>(new AppTaskDto(priorityDtoList, memberDtoList), false, "bir hata oluştu", null);
            }
            else
            {
                return new Result<AppTaskDto>(new AppTaskDto(priorityDtoList, memberDtoList), false, null, validationResult.Errors.ToMap());
            }
        }
    }
}
