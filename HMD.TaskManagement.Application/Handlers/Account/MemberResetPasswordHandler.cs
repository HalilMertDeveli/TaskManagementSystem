using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Security;
using HMD.TaskManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HMD.TaskManagement.Application.Handlers.Account
{
    public class MemberResetPasswordHandler : IRequestHandler<MemberResetPasswordRequest, Result<TemporaryPasswordDto?>>
    {
        private readonly IUserRepository repository;
        private readonly IPasswordHasher<AppUser> passwordHasher;

        public MemberResetPasswordHandler(IUserRepository repository, IPasswordHasher<AppUser> passwordHasher)
        {
            this.repository = repository;
            this.passwordHasher = passwordHasher;
        }

        public async Task<Result<TemporaryPasswordDto?>> Handle(MemberResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var updated = await repository.GetByFilterAsync(x => x.Id == request.Id && x.AppRoleId == (int)RoleType.Member, false);
            if (updated == null)
                return new Result<TemporaryPasswordDto?>(null, false, "Member bulunamadı", null);

            // Random one-time password instead of a fixed default; only its hash is stored.
            var temporaryPassword = PasswordSecurity.GenerateTemporaryPassword();
            updated.Password = passwordHasher.HashPassword(updated, temporaryPassword);

            var rows = await repository.SaveChangesAsync();

            if (rows > 0)
                return new Result<TemporaryPasswordDto?>(new TemporaryPasswordDto(updated.Id, updated.UserName, temporaryPassword), true, null, null);

            return new Result<TemporaryPasswordDto?>(null, false, "bir hata oluştu", null);
        }
    }
}
