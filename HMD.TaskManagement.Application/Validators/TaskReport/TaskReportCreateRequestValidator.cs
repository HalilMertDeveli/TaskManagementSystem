using FluentValidation;
using HMD.TaskManagement.Application.Requests;

namespace HMD.TaskManagement.Application.Validators.TaskReport
{
    public class TaskReportCreateRequestValidator : AbstractValidator<TaskReportCreateRequest>
    {
        public TaskReportCreateRequestValidator()
        {
            this.RuleFor(x => x.Detail).NotEmpty().WithMessage("Açıklama bilgisi boş olamaz");
            this.RuleFor(x => x.Definition).NotEmpty().WithMessage("Başlık bilgisi boş olamaz")
                .MaximumLength(250).WithMessage("Başlık en fazla 250 karakter olabilir");
            this.RuleFor(x => x.TaskId).GreaterThan(0).WithMessage("Geçersiz iş emri");
        }
    }
}
