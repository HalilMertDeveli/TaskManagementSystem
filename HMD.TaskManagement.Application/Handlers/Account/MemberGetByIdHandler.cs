using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.Account
{
    public class MemberGetByIdHandler : IRequestHandler<MemberGetByIdRequest, Result<MemberListDto>>
    {
        private readonly IUserRepository repository;

        public MemberGetByIdHandler(IUserRepository userRepository)
        {
            this.repository = userRepository;
        }

        public async Task<Result<MemberListDto?>> Handle(MemberGetByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await this.repository.GetByFilterAsync(x => x.Id == request.Id);
            if(result!=null)

                return new Result<MemberListDto?>(new MemberListDto(result.Id, result.Name, result.Surname, result.UserName), true, null, null);
            return new Result<MemberListDto?>(null, false, "", null);
        }
    }
}
