using FluentValidation;
using HMD.TaskManagement.Application.Requests;

namespace HMD.TaskManagement.Application.Validators.TaskReport
{
    public class TaskReportUpdateRequestValidator : AbstractValidator<TaskReportUpdateRequest>
    {
        public TaskReportUpdateRequestValidator()
        {
            this.RuleFor(x => x.Detail).NotEmpty().WithMessage("Açıklama bilgisi boş olamaz");
            this.RuleFor(x => x.Definition).NotEmpty().WithMessage("Başlık bilgisi boş olamaz")
                .MaximumLength(250).WithMessage("Başlık en fazla 250 karakter olabilir");
        }
    }
}
