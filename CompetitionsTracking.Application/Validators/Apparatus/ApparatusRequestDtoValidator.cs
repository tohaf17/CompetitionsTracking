using FluentValidation;
using CompetitionsTracking.Application.DTOs.Apparatus;

namespace CompetitionsTracking.Application.Validators.Apparatus
{
    public class ApparatusRequestDtoValidator : AbstractValidator<ApparatusRequestDto>
    {
        public ApparatusRequestDtoValidator()
        {
            // Add validation rules here
        }
    }
}
