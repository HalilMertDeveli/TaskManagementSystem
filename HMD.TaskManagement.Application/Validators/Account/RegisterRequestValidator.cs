using FluentValidation;
using HMD.TaskManagement.Application.Requests;
using HMD.TaskManagement.Application.Security;

namespace HMD.TaskManagement.Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Kullanıcı adı boş geçilemez")
                .MaximumLength(200).WithMessage("Kullanıcı adı en fazla 200 karakter olabilir");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Parola boş geçilemez")
                .Length(PasswordSecurity.MinimumLength, PasswordSecurity.MaximumLength)
                .WithMessage($"Parola {PasswordSecurity.MinimumLength}-{PasswordSecurity.MaximumLength} karakter olmalıdır");
            RuleFor(x => x.Name).NotEmpty().WithMessage("İsim kısmı boş geçilemez")
                .MaximumLength(200).WithMessage("İsim en fazla 200 karakter olabilir");
            RuleFor(x => x.Surname).NotEmpty().WithMessage("Soyisim boş geçilemez")
                .MaximumLength(200).WithMessage("Soyisim en fazla 200 karakter olabilir");
            RuleFor(x => x.Password).Equal(x => x.ConfirmPassword).WithMessage("Parolalar eşleşmiyor");
        }
    }
}
