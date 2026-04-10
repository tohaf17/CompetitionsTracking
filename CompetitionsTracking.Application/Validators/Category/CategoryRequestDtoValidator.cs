using FluentValidation;
using CompetitionsTracking.Application.DTOs.Category;

namespace CompetitionsTracking.Application.Validators.Category
{
    public class CategoryRequestDtoValidator : AbstractValidator<CategoryRequestDto>
    {
        public CategoryRequestDtoValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Category name/type is required.")
                .MaximumLength(100).WithMessage("Category name/type must not exceed 100 characters.");

            RuleFor(x => x.MinAge)
                .GreaterThanOrEqualTo(0).When(x => x.MinAge.HasValue)
                .WithMessage("Minimum age must be non-negative.");

            RuleFor(x => x.MaxAge)
                .GreaterThanOrEqualTo(x => x.MinAge ?? 0).When(x => x.MaxAge.HasValue && x.MinAge.HasValue)
                .WithMessage("Maximum age must be greater than or equal to minimum age.");
        }
    }

}
