using FluentValidation;
using CompetitionsTracking.Application.DTOs.Team;

namespace CompetitionsTracking.Application.Validators.Team
{
    public class TeamRequestDtoValidator : AbstractValidator<TeamRequestDto>
    {
        public TeamRequestDtoValidator()
        {
            // Add validation rules here
        }
    }
}
