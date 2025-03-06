using HMD.TaskManagement.Application.Dtos;
using MediatR;

namespace HMD.TaskManagement.Application.Requests
{
    public record AppTaskListRequest : PagedRequest, IRequest<PagedResult<AppTaskListDto>>
    {
        public AppTaskListRequest(int ActivePage) : base(ActivePage)
        {

        }
    }
}
