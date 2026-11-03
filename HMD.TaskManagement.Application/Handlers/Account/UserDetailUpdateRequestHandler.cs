using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Validators.Account;
using HMD.TaskManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HMD.TaskManagement.Application.Handlers.Account
{
    public class UserDetailUpdateRequestHandler : IRequestHandler<UserDetailUpdateRequest, Result<NoData>>
    {
        private readonly IUserRepository repository;
        private readonly IPasswordHasher<AppUser> passwordHasher;

        public UserDetailUpdateRequestHandler(IUserRepository repository, IPasswordHasher<AppUser> passwordHasher)
        {
            this.repository = repository;
            this.passwordHasher = passwordHasher;
        }

        public async Task<Result<NoData>> Handle(UserDetailUpdateRequest request, CancellationToken cancellationToken)
        {
            var validator = new UserDetailUpdateRequestValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return new Result<NoData>(new NoData(), false, null, validationResult.Errors.ToMap());
            }

            // request.Id is always taken from the authenticated user's claims by the controller.
            var updated = await this.repository.GetByFilterAsync(x => x.Id == request.Id, false);
            if (updated == null)
                return new Result<NoData>(new NoData(), false, "User bulunamadı", null);

            updated.Name = request.Name!;
            updated.Surname = request.Surname!;

            // The password is optional on the profile form: leaving it blank keeps the current one.
            if (!string.IsNullOrEmpty(request.Password))
            {
                updated.Password = passwordHasher.HashPassword(updated, request.Password);
            }

            await this.repository.SaveChangesAsync();
            return new Result<NoData>(new NoData(), true, null, null);
        }
    }
}
