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
    public class AppTaskListByUserIdHandler : IRequestHandler<AppTaskListByUserIdRequest, PagedResult<AppTaskListDto>>
    {
        private readonly IAppTaskRepository repository;

        public AppTaskListByUserIdHandler(IAppTaskRepository repository)
        {
            this.repository = repository;
        }

        public async Task<PagedResult<AppTaskListDto>> Handle(AppTaskListByUserIdRequest request, CancellationToken cancellationToken)
        {
            var list = await this.repository.GetAllByUserIdAsync(activePage: request.ActivePage, s: request.S, userId: request.UserId, pageSize: 7);

            var result = new List<AppTaskListDto>();

            foreach (var appTask in list.Data)
            {

                var dto = new AppTaskListDto(appTask.Id, appTask.Title, appTask.Description, appTask?.Priority?.Defination, appTask.State, appTask.AppUserId, appTask.AppUserId.HasValue ? appTask.AppUser?.Name + " " + appTask.AppUser?.Surname : null, appTask.PriorityId);
                result.Add(dto);
            }

            return new PagedResult<AppTaskListDto>(result, request.ActivePage, list.PageSize, list.TotalPages);
        }
    }
}
