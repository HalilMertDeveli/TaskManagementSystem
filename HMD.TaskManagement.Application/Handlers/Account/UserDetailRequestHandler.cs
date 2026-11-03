using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using MediatR;

namespace HMD.TaskManagement.Application.Handlers.Account
{
    public class UserDetailRequestHandler : IRequestHandler<UserDetailRequest, Result<UserDetailDto?>>
    {
        private readonly IUserRepository repository;

        public UserDetailRequestHandler(IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<UserDetailDto?>> Handle(UserDetailRequest request, CancellationToken cancellationToken)
        {
            var result = await this.repository.GetByFilterAsync(x => x.Id == request.Id);
            if (result == null)
                return new Result<UserDetailDto?>(null, false, "Kullanıcı bulunamadı", null);

            // The stored password (hash) is never returned to the UI.
            return new Result<UserDetailDto?>(new UserDetailDto(result.Id, result.Name, result.Surname), true, null, null);
        }
    }
}
