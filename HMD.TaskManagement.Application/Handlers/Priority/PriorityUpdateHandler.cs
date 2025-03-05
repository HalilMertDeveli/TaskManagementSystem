using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Validators.Priority;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers
{
    public class PriorityUpdateHandler:IRequestHandler<PriorityUpdateRequest,Result<NoData>>
    {
        private readonly IPriorityRepository repository;

        public PriorityUpdateHandler(IPriorityRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<NoData>> Handle(PriorityUpdateRequest request, CancellationToken cancellationToken)
        {
            var validator = new PriorityUpdateValidator();
            var validationResult = validator.Validate(request);
            if (validationResult.IsValid)
            {
                var updatedEntity = await this.repository.GetByFilterAsync(x => x.Id == request.Id);
                if (updatedEntity == null)
                {
                    return new Result<NoData>(new NoData(), false, "İlgili aciliyet bulunamadı", null);
                }

                updatedEntity.Defination = request.Definition ?? " ";
                await this.repository.SaveChangesAsync();
                return new Result<NoData>(new NoData(), true, null, null);
            }
            else
            {
                var errors = validationResult.Errors.ToMap();
                return new Result<NoData>(new NoData(), false, null, null);
            }

         

        }
    }
}
