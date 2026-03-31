using FluentValidation;
using CompetitionsTracking.Application.DTOs.Appeal;

namespace CompetitionsTracking.Application.Validators.Appeal
{
    public class AppealRequestDtoValidator : AbstractValidator<AppealRequestDto>
    {
        public AppealRequestDtoValidator()
        {
            // Add validation rules here
        }
    }
}
