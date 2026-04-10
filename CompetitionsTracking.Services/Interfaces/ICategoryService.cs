using CompetitionsTracking.Application.DTOs.Category;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto?> GetByIdAsync(int id);
        Task<CategoryResponseDto> CreateAsync(CategoryRequestDto request);
        Task UpdateAsync(int id, CategoryRequestDto request);
        Task DeleteAsync(int id);
        Task<CategoryStatsDto?> GetCategoryStatsAsync(int categoryId);
    }
}
