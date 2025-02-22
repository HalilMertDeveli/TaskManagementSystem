using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers
{
    public class LoginRequestHandler:IRequestHandler<LoginRequest,Result<LoginResponseDto>>
    {
        private readonly IUserRepository? userRepository;

        public LoginRequestHandler(IUserRepository? userRepository)
        {
            this.userRepository = userRepository;
        }

        public Task<Result<LoginResponseDto>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            //validation kuralları 
            throw new NotImplementedException();
        }
    }
}
