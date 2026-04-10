using FluentValidation;
using CompetitionsTracking.Application.DTOs.Appeal;

namespace CompetitionsTracking.Application.Validators.Appeal
{
    public class ApproveAppealRequestDtoValidator : AbstractValidator<ApproveAppealRequestDto>
    {
        public ApproveAppealRequestDtoValidator()
        {
            RuleFor(x => x.ScoreIdToEdit).GreaterThan(0).WithMessage("ScoreIdToEdit must be greater than 0.");
            RuleFor(x => x.NewScoreValue).InclusiveBetween(0, 30).WithMessage("New score value must be between 0 and 30.");
        }
    }
}
