using HMD.TaskManagement.Application.Dtos;
using MediatR;

namespace HMD.TaskManagement.Application.Requests
{
    public record NotificationListByUserIdRequest(int UserId) : IRequest<Result<List<NotificationListDto>>>;

    /// <summary>Marks a notification as read. Only succeeds for notifications addressed to <paramref name="UserId"/>.</summary>
    public record NotificationUpdateRequest(int Id, int UserId) : IRequest<Result<NoData>>;
}
