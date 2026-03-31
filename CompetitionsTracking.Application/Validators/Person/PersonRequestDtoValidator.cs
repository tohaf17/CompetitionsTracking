using FluentValidation;
using CompetitionsTracking.Application.DTOs.Person;

namespace CompetitionsTracking.Application.Validators.Person
{
    public class PersonRequestDtoValidator : AbstractValidator<PersonRequestDto>
    {
        public PersonRequestDtoValidator()
        {
            // Add validation rules here
        }
    }
}
