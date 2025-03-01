using HMD.TaskManagement.Application.Enums;

namespace HMD.TaskManagement.Application.Dtos
{
    public record LoginResponseDto(string Name, string Surname, RoleType Role);
}
