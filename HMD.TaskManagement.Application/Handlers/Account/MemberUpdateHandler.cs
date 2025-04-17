using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Validators.Account;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.Account
{
    public class MemberUpdateHandler : IRequestHandler<MemberUpdateRequest, Result<NoData>>
    {
        private readonly IUserRepository repository;

        public MemberUpdateHandler(IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<NoData>> Handle(MemberUpdateRequest request, CancellationToken cancellationToken)
        {
            var validator = new MemberUpdateRequestValidator();
            var validationResult = validator.Validate(request);
            if (validationResult.IsValid)
            {
                var updated = await repository.GetByFilterAsync(x => x.Id == request.Id, false);
                if (updated == null)
                    return new Result<NoData>(new NoData(), false, "Üye Bulunamadı", null);
                updated.Name = request.Name;
                updated.Surname = request.Surname;


                var rows = await repository.SaveChangesAsync();

                if (rows > 0)
                    return new Result<NoData>(new NoData(), true, null, null);

                return new Result<NoData>(new NoData(), false, "Bir hata oluştu", null);
            }
            else
            {
                return new  Result<NoData>(new NoData(), false, null, validationResult.Errors.ToMap());
            }
        }
    }
}
