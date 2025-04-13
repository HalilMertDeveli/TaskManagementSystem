namespace HMD.TaskManagement.Application.Dtos
{
    public record AppTaskListDto(int Id, string Title, string Description, string? PriorityDefinition, bool State);

    public record AppTaskCreateDto(string? Title, string? Description, int PriorityId);

    public record AppTaskDto(List<PriorityListDto> Priorities);

}
