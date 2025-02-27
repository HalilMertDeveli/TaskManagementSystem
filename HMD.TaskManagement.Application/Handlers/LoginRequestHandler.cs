using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Validators;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers
{
    public class LoginRequestHandler:IRequestHandler<LoginRequest,Result<LoginResponseDto?>>
    {
        private readonly IUserRepository? userRepository;

        public LoginRequestHandler(IUserRepository? userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Result<LoginResponseDto?>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var validator = new LoginRequestValidator();
           var validationResult= await validator.ValidateAsync(request);

            if (validationResult.IsValid)
            {
                return new Result<LoginResponseDto?>(new LoginResponseDto("", "", 1), true, null, null);
            }
            else
            {
                var errorList = new List<ValidationError>();
                var errors = validationResult.Errors.ToList();

                foreach (var error in errors)
                {
                    errorList.Add(new ValidationError(error.PropertyName,error.ErrorMessage));
                }

                return new Result<LoginResponseDto?>(null, false, "Bir Hata oluştu", errorList);

            }





        }
    }
}
