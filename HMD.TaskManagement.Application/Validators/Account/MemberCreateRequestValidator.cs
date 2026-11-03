using FluentValidation;
using HMD.TaskManagement.Application.Requests;

namespace HMD.TaskManagement.Application.Validators.Account
{
    public class MemberCreateRequestValidator : AbstractValidator<MemberCreateRequest>
    {
        public MemberCreateRequestValidator()
        {
            this.RuleFor(x => x.Username).NotEmpty().WithMessage("Kullanıcı Adı Boş Geçilemez")
                .MaximumLength(200).WithMessage("Kullanıcı adı en fazla 200 karakter olabilir");
            this.RuleFor(x => x.Name).NotEmpty().WithMessage("Ad boş geçilmez")
                .MaximumLength(200).WithMessage("Ad en fazla 200 karakter olabilir");
            this.RuleFor(x => x.Surname).NotEmpty().WithMessage("Soy ad boş geçilemez")
                .MaximumLength(200).WithMessage("Soyad en fazla 200 karakter olabilir");
        }
    }
}
