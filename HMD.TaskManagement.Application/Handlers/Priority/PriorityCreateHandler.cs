using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Validators;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers
{
    public class PriorityCreateHandler : IRequestHandler<PriorityCreateRequest, Result<NoData>>
    {
        private readonly IPriorityRepository repository;

        public PriorityCreateHandler(IPriorityRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<NoData>> Handle(PriorityCreateRequest request, CancellationToken cancellationToken)
        {
            var validator = new PriorityCreateRequestValidator();
            var validationResult = validator.Validate(request);
            if (validationResult.IsValid)
            {
                var rowCount = await this.repository.CreateAsync(request.ToMap());
                if (rowCount>0)
                {
                    return new Result<NoData>(new NoData(), true, null, null);
                }

                return new Result<NoData>(new NoData(), false, "Sistemsel hata öncelikler ",null);

            }
            else
            {
                var errors = validationResult.Errors.ToMap();
                return new Result<NoData>(new NoData(), false, null, errors);
            }
        }
    }
}
