using FluentValidation;
using CompetitionsTracking.Application.DTOs.Score;

namespace CompetitionsTracking.Application.Validators.Score
{
    public class ScoreRequestDtoValidator : AbstractValidator<ScoreRequestDto>
    {
        public ScoreRequestDtoValidator()
        {
            // Add validation rules here
        }
    }
}
