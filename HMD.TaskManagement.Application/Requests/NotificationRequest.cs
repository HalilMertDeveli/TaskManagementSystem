using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using MediatR;

namespace HMD.TaskManagement.Application.Requests
{
    public record NotificationListByUserIdRequest(int UserId) : IRequest<Result<List<NotificationListDto>>>;

    public record NotificationUpdateRequest(int Id) : IRequest<Result<NoData>>;
}
