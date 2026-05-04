using FluentValidation;
using CompetitionsTracking.Application.DTOs.Entry;

namespace CompetitionsTracking.Application.Validators.Entry
{
    public class EntryRequestDtoValidator : AbstractValidator<EntryRequestDto>
    {
        public EntryRequestDtoValidator()
        {
            RuleFor(x => x.CompetitionId).GreaterThan(0).WithMessage("CompetitionId must be greater than 0.");
            
            RuleFor(x => x.ParticipantId)
                .GreaterThan(0)
                .When(x => x.ParticipantId.HasValue && string.IsNullOrWhiteSpace(x.ParticipantName))
                .WithMessage("Необхідно обрати існуючого учасника або вказати ім'я для створення нового.");

            RuleFor(x => x.ParticipantName)
                .NotEmpty()
                .When(x => x.ParticipantId <= 0)
                .WithMessage("Ім'я учасника обов'язкове для нової заявки.");

            RuleFor(x => x.ParticipantSurname)
                .NotEmpty()
                .When(x => x.ParticipantId <= 0)
                .WithMessage("Прізвище учасника обов'язкове для нової заявки.");

            RuleFor(x => x.DisciplineId).GreaterThan(0).WithMessage("DisciplineId must be greater than 0.");
            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("CategoryId must be greater than 0.");
            
            RuleFor(x => x.ApplicationStatus)
                .IsInEnum()
                .When(x => x.ApplicationStatus.HasValue)
                .WithMessage("Invalid application status.");

            RuleFor(x => x.EntryStatus)
                .IsInEnum()
                .When(x => x.EntryStatus.HasValue)
                .WithMessage("Invalid entry status.");
        }
    }

}
