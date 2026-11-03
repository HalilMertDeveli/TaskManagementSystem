using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Validators;
using HMD.TaskManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HMD.TaskManagement.Application.Handlers
{
    public class RegisterRequestHandler : IRequestHandler<RegisterRequest, Result<NoData>>
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher<AppUser> passwordHasher;

        public RegisterRequestHandler(IUserRepository userRepository, IPasswordHasher<AppUser> passwordHasher)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
        }

        public async Task<Result<NoData>> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            var validator = new RegisterRequestValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return new Result<NoData>(new NoData(), false, null, validationResult.Errors.ToMap());
            }

            var existing = await userRepository.GetAllByFilterAsync(x => x.UserName == request.Username);
            if (existing.Count > 0)
            {
                return new Result<NoData>(new NoData(), false, null,
                    [new ValidationError(nameof(RegisterRequest.Username), "Bu kullanıcı adı kullanılamaz")]);
            }

            var user = request.ToMap();
            user.Password = passwordHasher.HashPassword(user, request.Password!);

            var rowCount = await userRepository.CreateUserAsync(user);
            if (rowCount > 0)
            {
                return new Result<NoData>(new NoData(), true, null, null);
            }
            return new Result<NoData>(new NoData(), false, "Bir sorun oluştu", null);
        }
    }
}
