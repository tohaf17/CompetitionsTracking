using FluentValidation;
using CompetitionsTracking.Application.DTOs.Competition;

namespace CompetitionsTracking.Application.Validators.Competition
{
    public class CompetitionFilterDtoValidator : AbstractValidator<CompetitionFilterDto>
    {
        public CompetitionFilterDtoValidator()
        {
            RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue).WithMessage("Invalid competition status filter.");
            RuleFor(x => x.City).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.City)).WithMessage("City filter must not exceed 100 characters.");
        }
    }
}
