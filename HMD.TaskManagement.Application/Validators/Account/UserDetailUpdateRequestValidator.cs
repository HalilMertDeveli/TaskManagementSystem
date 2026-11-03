using FluentValidation;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Security;

namespace HMD.TaskManagement.Application.Validators.Account
{
    public class UserDetailUpdateRequestValidator : AbstractValidator<UserDetailUpdateRequest>
    {
        public UserDetailUpdateRequestValidator()
        {
            // Password is optional (blank = keep current); when supplied it must meet the length policy.
            this.RuleFor(x => x.Password)
                .Length(PasswordSecurity.MinimumLength, PasswordSecurity.MaximumLength)
                .When(x => !string.IsNullOrEmpty(x.Password))
                .WithMessage($"Parola {PasswordSecurity.MinimumLength}-{PasswordSecurity.MaximumLength} karakter olmalıdır");
            this.RuleFor(x => x.Name).NotEmpty().WithMessage("Ad boş geçilemez")
                .MaximumLength(200).WithMessage("Ad en fazla 200 karakter olabilir");
            this.RuleFor(x => x.Surname).NotEmpty().WithMessage("Soyad boş geçilemez")
                .MaximumLength(200).WithMessage("Soyad en fazla 200 karakter olabilir");
        }
    }
}
