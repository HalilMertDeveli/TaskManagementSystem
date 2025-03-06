using FluentValidation;
using HMD.TaskManagement.Application.Requests;

namespace HMD.TaskManagement.Application.Validators.Priority
{
    public class PriorityUpdateValidator : AbstractValidator<PriorityUpdateRequest>
    {
        public PriorityUpdateValidator()
        {
            this.RuleFor(x => x.Definition).NotEmpty().WithMessage("Tanım bilgisi boş olamaz");
        }
    }
}
