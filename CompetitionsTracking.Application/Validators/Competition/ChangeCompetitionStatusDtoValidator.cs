using FluentValidation;
using CompetitionsTracking.Application.DTOs.Competition;

namespace CompetitionsTracking.Application.Validators.Competition
{
    public class ChangeCompetitionStatusDtoValidator : AbstractValidator<ChangeCompetitionStatusDto>
    {
        public ChangeCompetitionStatusDtoValidator()
        {
            RuleFor(x => x.NewStatus).IsInEnum().WithMessage("Invalid competition status.");
        }
    }
}
