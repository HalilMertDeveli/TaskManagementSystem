using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers
{
    public class AppTaskListHandler:IRequestHandler<AppTaskListRequest,Result<List<AppTaskListDto>>>
    {
        private readonly IAppTaskRepository repository;

        public AppTaskListHandler(IAppTaskRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<List<AppTaskListDto>>> Handle(AppTaskListRequest request, CancellationToken cancellationToken)
        {
            var list = await this.repository.GetAllAsync();
            var result = new List<AppTaskListDto>();

            foreach (var appTask in list)
            {
                var dto = new AppTaskListDto(appTask.Id, appTask.Title, appTask.Description,
                    appTask?.Priority?.Defination, appTask.State);
                result.Add(dto);
            }

            return new Result<List<AppTaskListDto>>(result,true,null,null);
        }
    }
}
