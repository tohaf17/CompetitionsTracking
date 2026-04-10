using FluentValidation;
using CompetitionsTracking.Application.DTOs.Entry;

namespace CompetitionsTracking.Application.Validators.Entry
{
    public class TransferEntryDtoValidator : AbstractValidator<TransferEntryDto>
    {
        public TransferEntryDtoValidator()
        {
            RuleFor(x => x.NewCategoryId).GreaterThan(0).WithMessage("NewCategoryId must be greater than 0.");
            RuleFor(x => x.NewDisciplineId).GreaterThan(0).WithMessage("NewDisciplineId must be greater than 0.");
        }
    }
}
