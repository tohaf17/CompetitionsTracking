using FluentValidation;
using CompetitionsTracking.Application.DTOs.Judge;

namespace CompetitionsTracking.Application.Validators.Judge
{
    public class JudgeRequestDtoValidator : AbstractValidator<JudgeRequestDto>
    {
        public JudgeRequestDtoValidator()
        {
            // Add validation rules here
        }
    }
}
