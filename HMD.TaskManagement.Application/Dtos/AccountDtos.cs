using HMD.TaskManagement.Application.Enums;

namespace HMD.TaskManagement.Application.Dtos
{
    public record LoginResponseDto(string Name, string Surname, RoleType Role,int Id);

    public record MemberListDto(int Id, string Name, string Surname, string Username);
    public record UserDetailDto(int Id, string Name, string Surname, string Password);

}
