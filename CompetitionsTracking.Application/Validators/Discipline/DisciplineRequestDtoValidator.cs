using FluentValidation;
using CompetitionsTracking.Application.DTOs.Discipline;

namespace CompetitionsTracking.Application.Validators.Discipline
{
    public class DisciplineRequestDtoValidator : AbstractValidator<DisciplineRequestDto>
    {
        public DisciplineRequestDtoValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Discipline name/type is required.")
                .MaximumLength(100).WithMessage("Discipline name/type must not exceed 100 characters.");

            RuleFor(x => x.ApparatusId).GreaterThan(0).WithMessage("ApparatusId must be greater than 0.");
        }
    }

}
