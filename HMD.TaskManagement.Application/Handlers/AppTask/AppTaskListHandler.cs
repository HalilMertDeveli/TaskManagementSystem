using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers
{
    public class AppTaskListHandler : IRequestHandler<AppTaskListRequest, PagedResult<AppTaskListDto>>
    {
        private readonly IAppTaskRepository repository;

        public AppTaskListHandler(IAppTaskRepository repository)
        {
            this.repository = repository;
        }

        public async Task<PagedResult<AppTaskListDto>> Handle(AppTaskListRequest request, CancellationToken cancellationToken)
        {
            var list = await this.repository.GetAllAsync(activePage: request.ActivePage, pageSize: 5, s: request.S);
            var result = new List<AppTaskListDto>();

            foreach (var appTask in list.Data)
            {
                var dto = new AppTaskListDto(appTask.Id, appTask.Title, appTask.Description,
                    appTask?.Priority?.Defination, appTask.State);
                result.Add(dto);
            }

            return new PagedResult<AppTaskListDto>(result, request.ActivePage, list.PageSize, list.TotalPages);
        }
    }
}
