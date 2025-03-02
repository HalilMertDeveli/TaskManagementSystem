using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers
{
    public class PriorityListHandler : IRequestHandler<PriorityListRequest, Result<List<PriorityListDto>>>
    {
        private readonly IPriorityRepository repository;

        public PriorityListHandler(IPriorityRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<List<PriorityListDto>>> Handle(PriorityListRequest request, CancellationToken cancellationToken)
        {
            var result = await this.repository.GetAllAsync();

            var mappedResult = result.Select(x => new PriorityListDto(x.Id, x.Defination)).ToList();
            return new Result<List<PriorityListDto>>(mappedResult, true, null, null);

        }
    }
}
