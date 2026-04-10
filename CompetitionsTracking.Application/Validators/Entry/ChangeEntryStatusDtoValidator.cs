using FluentValidation;
using CompetitionsTracking.Application.DTOs.Entry;

namespace CompetitionsTracking.Application.Validators.Entry
{
    public class ChangeEntryStatusDtoValidator : AbstractValidator<ChangeEntryStatusDto>
    {
        public ChangeEntryStatusDtoValidator()
        {
            RuleFor(x => x.NewStatus).IsInEnum().WithMessage("Invalid entry status.");
        }
    }
}
