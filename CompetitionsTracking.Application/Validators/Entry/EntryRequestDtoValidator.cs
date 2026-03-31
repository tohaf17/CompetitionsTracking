using FluentValidation;
using CompetitionsTracking.Application.DTOs.Entry;

namespace CompetitionsTracking.Application.Validators.Entry
{
    public class EntryRequestDtoValidator : AbstractValidator<EntryRequestDto>
    {
        public EntryRequestDtoValidator()
        {
            // Add validation rules here
        }
    }
}
