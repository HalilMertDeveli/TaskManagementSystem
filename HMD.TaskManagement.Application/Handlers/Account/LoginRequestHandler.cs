using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Security;
using HMD.TaskManagement.Application.Validators;
using HMD.TaskManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HMD.TaskManagement.Application.Handlers
{
    public class LoginRequestHandler : IRequestHandler<LoginRequest, Result<LoginResponseDto?>>
    {
        private const string InvalidCredentialsMessage = "Kullanıcı adı veya şifre hatalı";

        // Used to spend comparable time when the user name does not exist (reduces user enumeration via timing).
        private static readonly Lazy<string> DummyHash =
            new(() => new PasswordHasher<AppUser>().HashPassword(new AppUser(), Guid.NewGuid().ToString("N")));

        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher<AppUser> passwordHasher;

        public LoginRequestHandler(IUserRepository userRepository, IPasswordHasher<AppUser> passwordHasher)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
        }

        public async Task<Result<LoginResponseDto?>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var validator = new LoginRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return new Result<LoginResponseDto?>(null, false, null, validationResult.Errors.ToMap());
            }

            // The user name is not enforced unique in the schema, so every candidate is checked.
            var candidates = await userRepository.GetAllByFilterAsync(x => x.UserName == request.Username, asNoTracking: false);

            if (candidates.Count == 0)
            {
                passwordHasher.VerifyHashedPassword(new AppUser(), DummyHash.Value, request.Password!);
                return new Result<LoginResponseDto?>(null, false, InvalidCredentialsMessage, null);
            }

            foreach (var user in candidates)
            {
                var (success, needsRehash) = PasswordSecurity.Verify(passwordHasher, user, request.Password!);
                if (!success)
                    continue;

                if (needsRehash)
                {
                    // Transparent migration: a legacy plain-text value (or an outdated hash) is replaced by a current hash.
                    user.Password = passwordHasher.HashPassword(user, request.Password!);
                    await userRepository.SaveChangesAsync();
                }

                var type = (RoleType)user.AppRoleId;
                return new Result<LoginResponseDto?>(new LoginResponseDto(user.Name, user.Surname, type, user.Id), true, null, null);
            }

            return new Result<LoginResponseDto?>(null, false, InvalidCredentialsMessage, null);
        }
    }
}
