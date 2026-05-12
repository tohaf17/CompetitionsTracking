using FluentValidation;
using CompetitionsTracking.Application.DTOs.Competition;

namespace CompetitionsTracking.Application.Validators.Competition
{
    public class CompetitionRequestDtoValidator : AbstractValidator<CompetitionRequestDto>
    {
        public CompetitionRequestDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Competition title is required.")
                .MaximumLength(200).WithMessage("Competition title must not exceed 200 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City name must not exceed 100 characters.");

            RuleFor(x => x.Level)
                .IsInEnum().WithMessage("Competition level is invalid.");

            RuleFor(x => x.Country)
                .MaximumLength(100).WithMessage("Country name must not exceed 100 characters.")
                .NotEmpty().WithMessage("Country is required for international competitions.")
                .When(x => x.Level == Domain.Entities.CompetitionLevel.International);

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be greater than or equal to start date.");
        }
    }

}
