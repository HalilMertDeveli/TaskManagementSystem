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
        private readonly IAppTaskRepository apptaskRepository;
        private readonly IPriorityRepository priorityRepository;
        private readonly IUserRepository userRepository;

        public AppTaskUpdateHandler(IUserRepository userRepository, IPriorityRepository priorityRepository, IAppTaskRepository apptaskRepository)
        {
            this.userRepository = userRepository;
            this.priorityRepository = priorityRepository;
            this.apptaskRepository = apptaskRepository;
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
                var updated = await this.apptaskRepository.GetByFilterAsync(x => x.Id == request.Id);
                if (updated == null)
                {
                    return new Result<AppTaskDto>(new AppTaskDto(priorityDtoList, memberDtoList), false,
                        "Task Bulunamadı", null);

                }

                if (request.AppUserId == null || request.AppUserId == 0)
                {
                    updated.AppUserId = null;
                }
                else
                {
                    updated.AppUserId = request.AppUserId;
                }

                updated.Title = request.Title;
                updated.Description = request.Description;
                updated.PriorityId = request.PriorityId;

                var rows = await this.apptaskRepository.SaveChangesAsync();



                if (rows > 0)

                    return new Result<AppTaskDto>(new AppTaskDto(priorityDtoList,memberDtoList), true, null, null);



                return new Result<AppTaskDto>(new AppTaskDto(priorityDtoList, memberDtoList), false, "Veritabanı hatası var, veritabanı tarafında", null);

            }
            else
            {
                return new Result<AppTaskDto>(new AppTaskDto(priorityDtoList, memberDtoList), false, null, validationResult.Errors.ToMap());

            }
        }
    }
}
