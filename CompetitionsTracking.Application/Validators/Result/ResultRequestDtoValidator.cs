using FluentValidation;
using CompetitionsTracking.Application.DTOs.Result;

namespace CompetitionsTracking.Application.Validators.Result
{
    public class ResultRequestDtoValidator : AbstractValidator<ResultRequestDto>
    {
        public ResultRequestDtoValidator()
        {
            // Add validation rules here
        }
    }
}
