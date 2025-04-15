using HMD.TaskManagement.Domain.Entities;

namespace HMD.TaskManagement.Application.Dtos
{
    public record AppTaskListDto(int Id, string Title, string Description, string? PriorityDefinition, bool State, int? AppUserId, string? AppUserFullName,int PriorityId);

    public record AppTaskCreateDto(string? Title, string? Description, int PriorityId);

    public record AppTaskDto(List<PriorityListDto> Priorities,List<MemberListDto>? Employees=null);

}
