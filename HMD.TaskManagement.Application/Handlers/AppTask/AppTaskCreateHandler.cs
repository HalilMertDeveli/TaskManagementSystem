using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Validators;
using HMD.TaskManagement.Domain.Entities;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.AppTask
{
    public class AppTaskCreateHandler:IRequestHandler<AppTaskCreateRequest,Result<AppTaskDto>>
    {
        private readonly IAppTaskRepository repository;
        private readonly IPriorityRepository priorityRepository;

        public AppTaskCreateHandler(IAppTaskRepository repository, IPriorityRepository priorityRepository)
        {
            this.repository = repository;
            this.priorityRepository = priorityRepository;
        }

        public async Task<Result<AppTaskDto>> Handle(AppTaskCreateRequest request, CancellationToken cancellationToken)
        {
            var validator = new AppTaskCreateRequestValidator();
            var validationResult = validator.Validate(request);

            var priorities = await this.priorityRepository.GetAllAsync();

            var priorityDtoList = priorities.Select(x => new PriorityListDto(x.Id, x.Defination)).ToList();

            if (validationResult.IsValid)
            {
                var effectedRow = await this.repository.CreateAsync(request.ToMap());

                if (effectedRow>0)
                
                    return new Result<AppTaskDto>(new AppTaskDto(priorityDtoList), true, null, null);

                

                return new Result<AppTaskDto>(new AppTaskDto(priorityDtoList), false, "Veritabanı hatası var, veritabanı tarafında", null);

            }
            else
            {
                return new Result<AppTaskDto>(new AppTaskDto(priorityDtoList), false, null, validationResult.Errors.ToMap());
               
            }


           
        }
    }
}
