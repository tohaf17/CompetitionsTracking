using FluentValidation;
using CompetitionsTracking.Application.DTOs.Entry;

namespace CompetitionsTracking.Application.Validators.Entry
{
    public class BulkUpdateAppStatusDtoValidator : AbstractValidator<BulkUpdateAppStatusDto>
    {
        public BulkUpdateAppStatusDtoValidator()
        {
            RuleFor(x => x.CompetitionId).GreaterThan(0).WithMessage("CompetitionId must be greater than 0.");
            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("CategoryId must be greater than 0.");
            RuleFor(x => x.NewStatus).IsInEnum().WithMessage("Invalid application status.");
        }
    }
}
