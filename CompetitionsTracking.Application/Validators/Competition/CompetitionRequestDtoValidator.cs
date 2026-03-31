using FluentValidation;
using CompetitionsTracking.Application.DTOs.Competition;

namespace CompetitionsTracking.Application.Validators.Competition
{
    public class CompetitionRequestDtoValidator : AbstractValidator<CompetitionRequestDto>
    {
        public CompetitionRequestDtoValidator()
        {
            // Add validation rules here
        }
    }
}
