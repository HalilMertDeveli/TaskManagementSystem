using HMD.TaskManagement.Application.Dtos;
using MediatR;

namespace HMD.TaskManagement.Application.Requests
{
    public record LoginRequest(string? Username, string? Password, bool RememberMe = false) : IRequest<Result<LoginResponseDto?>>;

    public record RegisterRequest(string? Username, string? Password, string? ConfirmPassword, string? Name, string? Surname)
        : IRequest<Result<NoData>>;

    public record MemberListRequest() : IRequest<Result<List<MemberListDto>>>;
}
