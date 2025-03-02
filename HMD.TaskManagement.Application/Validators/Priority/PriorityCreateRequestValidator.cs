using FluentValidation;
using HMD.TaskManagement.Application.Requests;

namespace HMD.TaskManagement.Application.Validators
{
    public class PriorityCreateRequestValidator : AbstractValidator<PriorityCreateRequest>
    {
        public PriorityCreateRequestValidator()
        {
            this.RuleFor(x => x.Definition).NotEmpty().WithMessage("Tanım Alanı Boş Bırakılamaz");
        }
    }
}
