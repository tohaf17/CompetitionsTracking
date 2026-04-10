using FluentValidation;
using CompetitionsTracking.Application.DTOs.Apparatus;

namespace CompetitionsTracking.Application.Validators.Apparatus
{
    public class ApparatusRequestDtoValidator : AbstractValidator<ApparatusRequestDto>
    {
        public ApparatusRequestDtoValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Apparatus type name is required.")
                .MaximumLength(50).WithMessage("Apparatus type name must not exceed 50 characters.");
        }
    }

}
