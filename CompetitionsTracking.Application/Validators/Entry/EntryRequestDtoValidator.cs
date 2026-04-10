using FluentValidation;
using CompetitionsTracking.Application.DTOs.Entry;

namespace CompetitionsTracking.Application.Validators.Entry
{
    public class EntryRequestDtoValidator : AbstractValidator<EntryRequestDto>
    {
        public EntryRequestDtoValidator()
        {
            RuleFor(x => x.CompetitionId).GreaterThan(0).WithMessage("CompetitionId must be greater than 0.");
            RuleFor(x => x.ParticipantId).GreaterThan(0).WithMessage("ParticipantId must be greater than 0.");
            RuleFor(x => x.DisciplineId).GreaterThan(0).WithMessage("DisciplineId must be greater than 0.");
            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("CategoryId must be greater than 0.");
            RuleFor(x => x.ApplicationStatus).IsInEnum().WithMessage("Invalid application status.");
            RuleFor(x => x.EntryStatus).IsInEnum().WithMessage("Invalid entry status.");
        }
    }

}
