using HMD.TaskManagement.Application.Dtos;
using MediatR;

namespace HMD.TaskManagement.Application.Requests
{

    public record TaskReportGetByTaskIdRequest(int Id) : IRequest<Result<List<TaskReportListDto>>>;

    // Member-side report operations. UserId is always set by the controller from the signed-in user's
    // claims (never from form data); handlers only act on reports whose task is assigned to that user.

    public record TaskReportGetByIdRequest(int Id, int UserId) : IRequest<Result<TaskReportListDto?>>;

    public record TaskReportCreateRequest(string? Detail, string? Definition, int TaskId, int UserId = 0) : IRequest<Result<NoData>>;

    public record TaskReportUpdateRequest(int Id, string? Detail, string? Definition, int TaskId, int UserId = 0) : IRequest<Result<NoData>>;

    public record TaskReportDeleteRequest(int Id, int UserId) : IRequest<Result<NoData>>;


}
