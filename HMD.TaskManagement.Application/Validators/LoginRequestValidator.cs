using FluentValidation;
using HMD.TaskManagement.Application.Requests;

namespace HMD.TaskManagement.Application.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            this.RuleFor(x => x.Username).NotEmpty().WithMessage("Kullanıcı adı boş geçilemez");
            this.RuleFor(x => x.Password).NotEmpty().WithMessage("Parola boş geçilemez");
        }
    }
}
