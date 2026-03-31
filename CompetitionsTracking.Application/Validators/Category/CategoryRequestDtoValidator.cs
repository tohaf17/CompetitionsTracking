using FluentValidation;
using CompetitionsTracking.Application.DTOs.Category;

namespace CompetitionsTracking.Application.Validators.Category
{
    public class CategoryRequestDtoValidator : AbstractValidator<CategoryRequestDto>
    {
        public CategoryRequestDtoValidator()
        {
            // Add validation rules here
        }
    }
}
