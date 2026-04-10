using FluentValidation;
using CompetitionsTracking.Application.DTOs.Judge;

namespace CompetitionsTracking.Application.Validators.Judge
{
    public class JudgeRequestDtoValidator : AbstractValidator<JudgeRequestDto>
    {
        public JudgeRequestDtoValidator()
        {
            RuleFor(x => x.PersonId).GreaterThan(0).WithMessage("PersonId must be greater than 0.");
            RuleFor(x => x.QualificationLevel)
                .NotEmpty().WithMessage("Qualification level is required.")
                .MaximumLength(50).WithMessage("Qualification level must not exceed 50 characters.");
        }
    }

}
