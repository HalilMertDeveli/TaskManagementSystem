using HMD.TaskManagement.Application.Dtos;
using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Application.Extensions;
using HMD.TaskManagement.Application.Interfaces;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Validators;
using MediatR;
using System.Text.Json;

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
            // #region agent log
            try { File.AppendAllText("C:/Users/clt/Desktop/TaskManagementSystem/debug-8b40ae.log", JsonSerializer.Serialize(new { sessionId = "8b40ae", runId = "investigation-1", hypothesisId = "H3", location = "LoginRequestHandler.cs:Handle:start", message = "Login handler invoked", data = new { hasRepository = userRepository != null, username = request.Username }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + Environment.NewLine); } catch { }
            // #endregion

            var validator = new LoginRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.IsValid)
            {
                // #region agent log
                try { File.AppendAllText("C:/Users/clt/Desktop/TaskManagementSystem/debug-8b40ae.log", JsonSerializer.Serialize(new { sessionId = "8b40ae", runId = "investigation-1", hypothesisId = "H4", location = "LoginRequestHandler.cs:Handle:query", message = "Attempting user lookup", data = new { isValid = true }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + Environment.NewLine); } catch { }
                // #endregion

                var user = await userRepository.GetByFilterAsync(x =>
                    x.Password == request.Password && x.UserName == request.Username);




                if (user != null)
                {
                    // #region agent log
                    try { File.AppendAllText("C:/Users/clt/Desktop/TaskManagementSystem/debug-8b40ae.log", JsonSerializer.Serialize(new { sessionId = "8b40ae", runId = "investigation-1", hypothesisId = "H5", location = "LoginRequestHandler.cs:Handle:success", message = "User found for login", data = new { userId = user.Id, roleId = user.AppRoleId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + Environment.NewLine); } catch { }
                    // #endregion

                    var type = (RoleType)user.AppRoleId;
                    return new Result<LoginResponseDto?>(new LoginResponseDto(user.Name, user.Surname, type,user.Id),
                        true, null, null);

                }
                else
                {
                    // #region agent log
                    try { File.AppendAllText("C:/Users/clt/Desktop/TaskManagementSystem/debug-8b40ae.log", JsonSerializer.Serialize(new { sessionId = "8b40ae", runId = "investigation-1", hypothesisId = "H5", location = "LoginRequestHandler.cs:Handle:notfound", message = "No user matched login request", data = new { username = request.Username }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + Environment.NewLine); } catch { }
                    // #endregion

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
