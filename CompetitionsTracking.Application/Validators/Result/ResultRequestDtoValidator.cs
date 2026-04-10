using FluentValidation;
using CompetitionsTracking.Application.DTOs.Result;

namespace CompetitionsTracking.Application.Validators.Result
{
    public class ResultRequestDtoValidator : AbstractValidator<ResultRequestDto>
    {
        public ResultRequestDtoValidator()
        {
            RuleFor(x => x.EntryId).GreaterThan(0).WithMessage("EntryId must be greater than 0.");
            RuleFor(x => x.Place).GreaterThan(0).WithMessage("Place must be a positive integer.");
            RuleFor(x => x.FinalScore).InclusiveBetween(0, 100).WithMessage("Final score must be between 0 and 100.");
            RuleFor(x => x.AwardedMedal).MaximumLength(50).WithMessage("Awarded medal name must not exceed 50 characters.");
        }
    }

}
