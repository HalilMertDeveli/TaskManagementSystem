using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Security;
using HMD.TaskManagement.Application.Validators.Account;
using HMD.TaskManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HMD.TaskManagement.Application.Handlers.Account
{
    public class MemberCreateHandler : IRequestHandler<MemberCreateRequest, Result<TemporaryPasswordDto?>>
    {
        private readonly IUserRepository repository;
        private readonly IPasswordHasher<AppUser> passwordHasher;

        public MemberCreateHandler(IUserRepository repository, IPasswordHasher<AppUser> passwordHasher)
        {
            this.repository = repository;
            this.passwordHasher = passwordHasher;
        }

        public async Task<Result<TemporaryPasswordDto?>> Handle(MemberCreateRequest request, CancellationToken cancellationToken)
        {
            var validator = new MemberCreateRequestValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return new Result<TemporaryPasswordDto?>(null, false, null, validationResult.Errors.ToMap());
            }

            var existing = await repository.GetAllByFilterAsync(x => x.UserName == request.Username);
            if (existing.Count > 0)
            {
                return new Result<TemporaryPasswordDto?>(null, false, null,
                    [new ValidationError(nameof(MemberCreateRequest.Username), "Bu kullanıcı adı kullanılamaz")]);
            }

            // No shared default password: every new member gets a random one-time password,
            // shown once to the admin and stored only as a hash.
            var temporaryPassword = PasswordSecurity.GenerateTemporaryPassword();

            var user = new AppUser
            {
                AppRoleId = (int)RoleType.Member,
                Name = request.Name!,
                Surname = request.Surname!,
                UserName = request.Username!,
            };
            user.Password = passwordHasher.HashPassword(user, temporaryPassword);

            var rows = await repository.CreateUserAsync(user);
            if (rows > 0)
                return new Result<TemporaryPasswordDto?>(new TemporaryPasswordDto(user.Id, user.UserName, temporaryPassword), true, null, null);

            return new Result<TemporaryPasswordDto?>(null, false, "Bir hata oluştu", null);
        }
    }
}
