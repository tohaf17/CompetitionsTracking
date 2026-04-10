using FluentValidation;
using CompetitionsTracking.Application.DTOs.Person;

namespace CompetitionsTracking.Application.Validators.Person
{
    public class PersonRequestDtoValidator : AbstractValidator<PersonRequestDto>
    {
        public PersonRequestDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Surname is required.")
                .MaximumLength(100).WithMessage("Surname must not exceed 100 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past.");

            RuleFor(x => x.Gender).IsInEnum().WithMessage("Invalid gender.");

            RuleFor(x => x.MentorId)
                .GreaterThan(0).When(x => x.MentorId.HasValue)
                .WithMessage("MentorId must be greater than 0 if provided.");
        }
    }

}
