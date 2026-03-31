using FluentValidation;
using CompetitionsTracking.Application.DTOs.Discipline;

namespace CompetitionsTracking.Application.Validators.Discipline
{
    public class DisciplineRequestDtoValidator : AbstractValidator<DisciplineRequestDto>
    {
        public DisciplineRequestDtoValidator()
        {
            // Add validation rules here
        }
    }
}
