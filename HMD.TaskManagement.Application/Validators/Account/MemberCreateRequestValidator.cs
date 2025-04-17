using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using HMD.TaskManagement.Application.Requests;

namespace HMD.TaskManagement.Application.Validators.Account
{
    public class MemberCreateRequestValidator:AbstractValidator<MemberCreateRequest>
    {
        public MemberCreateRequestValidator()
        {
            this.RuleFor(x => x.Username).NotEmpty().WithMessage("Kullanıcı Adı Boş Geçilemez");
            this.RuleFor(x => x.Name).NotEmpty().WithMessage("Ad boş geçilmez");
            this.RuleFor(x => x.Surname).NotEmpty().WithMessage("Soy ad boş geçilemez");
        }
    }
}
