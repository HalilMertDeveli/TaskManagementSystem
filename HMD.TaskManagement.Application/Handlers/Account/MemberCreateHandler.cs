using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Validators.Account;
using HMD.TaskManagement.Domain.Entities;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.Account
{
    public class MemberCreateHandler:IRequestHandler<MemberCreateRequest,Result<NoData>>
    {
        private readonly IUserRepository repository;

        public MemberCreateHandler(IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<NoData>> Handle(MemberCreateRequest request, CancellationToken cancellationToken)
        {
            var validator = new MemberCreateRequestValidator();
            var validationResult = validator.Validate(request);

            if (validationResult.IsValid)
            {
                var rows = await this.repository.CreateUserAsync(new AppUser()
                {
                    AppRoleId = (int)RoleType.Member,
                    Name = request.Name,
                    Password = "123",
                    Surname = request.Surname,
                    UserName = request.Username,
                });
                if (rows>0)
                
                    return new Result<NoData>(new NoData(), true, null, null);

                

                return new Result<NoData>(new NoData(), false, "Bir hata oluştu", null);
            }
            else
            {
                return new Result<NoData>(new NoData(), false, null, validationResult.Errors.ToMap());
            }



           
        }
    }
}
