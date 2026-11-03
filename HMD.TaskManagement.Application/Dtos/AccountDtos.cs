using HMD.TaskManagement.Application.Enums;

namespace HMD.TaskManagement.Application.Dtos
{
    public record LoginResponseDto(string Name, string Surname, RoleType Role,int Id);

    public record MemberListDto(int Id, string Name, string Surname, string Username);
    public record UserDetailDto(int Id, string Name, string Surname);

    /// <summary>One-time password generated for an admin-initiated member create / reset. Shown once, never stored in clear.</summary>
    public record TemporaryPasswordDto(int UserId, string Username, string TemporaryPassword);

}
