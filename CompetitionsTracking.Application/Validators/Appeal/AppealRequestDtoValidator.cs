using FluentValidation;
using CompetitionsTracking.Application.DTOs.Appeal;

namespace CompetitionsTracking.Application.Validators.Appeal
{
    public class AppealRequestDtoValidator : AbstractValidator<AppealRequestDto>
    {
        public AppealRequestDtoValidator()
        {
            RuleFor(x => x.ResultId).GreaterThan(0).WithMessage("ResultId must be greater than 0.");
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Appeal reason is required.")
                .MaximumLength(500).WithMessage("Appeal reason must not exceed 500 characters.");
        }
    }

}
