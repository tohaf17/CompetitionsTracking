using FluentValidation;
using CompetitionsTracking.Application.DTOs.Team;

namespace CompetitionsTracking.Application.Validators.Team
{
    public class TeamRequestDtoValidator : AbstractValidator<TeamRequestDto>
    {
        public TeamRequestDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Team name is required.")
                .MaximumLength(100).WithMessage("Team name must not exceed 100 characters.");

            RuleFor(x => x.CoachId).GreaterThan(0).WithMessage("CoachId must be greater than 0.");
        }
    }

}
