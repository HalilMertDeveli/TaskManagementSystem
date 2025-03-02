using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Validators;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers
{
    public class LoginRequestHandler : IRequestHandler<LoginRequest, Result<LoginResponseDto?>>
    {
        private readonly IUserRepository? userRepository;

        public LoginRequestHandler(IUserRepository? userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Result<LoginResponseDto?>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var validator = new LoginRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.IsValid)
            {
                var user = await userRepository.GetByFilterAsync(x =>
                    x.Password == request.Password && x.UserName == request.Username);




                if (user != null)
                {
                    var type = (RoleType)user.AppRoleId;
                    return new Result<LoginResponseDto?>(new LoginResponseDto(user.Name, user.Surname, type),
                        true, null, null);

                }
                else
                {
                    return new Result<LoginResponseDto?>(null, false, "Kullanıcı adı veya şifre hatalı", null);

                }

            }
            else
            {
                var errorList = validationResult.Errors.ToMap();

                return new Result<LoginResponseDto?>(null, false, null, errorList);

            }





        }
    }
}
