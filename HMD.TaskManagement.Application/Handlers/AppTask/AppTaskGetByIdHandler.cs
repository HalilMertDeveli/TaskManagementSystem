using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.AppTask
{
    public class AppTaskGetByIdHandler:IRequestHandler<AppTaskGetByIdRequest,Result<AppTaskListDto>>
    {

        private readonly IAppTaskRepository repository;

        public AppTaskGetByIdHandler(IAppTaskRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<AppTaskListDto>> Handle(AppTaskGetByIdRequest request, CancellationToken cancellationToken)
        {
            

            var result= await this.repository.GetByFilterAsNoTracking(x => x.Id == request.Id);
            return new Result<AppTaskListDto>(result.ToMap(), true, null, null);
        }
    }
}
