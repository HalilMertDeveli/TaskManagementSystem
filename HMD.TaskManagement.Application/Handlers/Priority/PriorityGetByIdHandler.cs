using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers
{
    public class PriorityGetByIdHandler : IRequestHandler<PriorityGetByIdRequest, Result<PriorityListDto>>
    {
        private readonly IPriorityRepository repository;

        public PriorityGetByIdHandler(IPriorityRepository repository)
        {
            this.repository = repository;
        }


        public async Task<Result<PriorityListDto?>> Handle(PriorityGetByIdRequest request, CancellationToken cancellationToken)
        {
            var priority = await this.repository.GetByFilterAsNoTrackingAsync(x => x.Id == request.Id);
            if (priority != null)
            {
                return new Result<PriorityListDto?>(new PriorityListDto(priority.Id, priority.Defination), true, null,
                    null);
            }

            return new Result<PriorityListDto?>(null, false, "Priority bulunamadı", null);

        }
    }

}
