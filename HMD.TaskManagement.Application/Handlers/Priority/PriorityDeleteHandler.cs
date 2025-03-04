using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;

namespace HMD.TaskManagement.Application.Handlers.Priority
{
    public class PriorityDeleteHandler : IRequestHandler<PriorityDeleteRequest,Result<NoData>>
    {
        private readonly IPriorityRepository repository;

        public PriorityDeleteHandler(IPriorityRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<NoData>> Handle(PriorityDeleteRequest request, CancellationToken cancellationToken)
        {
            var deletedEntity = await this.repository.GetByFilterAsync(x => x.Id == request.Id);
            if(deletedEntity != null)
            {
                await this.repository.DeleteAsync(deletedEntity);
                return new Result<NoData>(new NoData(), true, null, null);
            }
            return new Result<NoData>(new NoData(), false, "Sistem yöneticisine başvur", null);


        }
    }
}
