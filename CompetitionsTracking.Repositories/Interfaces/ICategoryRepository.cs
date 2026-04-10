using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<CompetitionsTracking.Application.DTOs.Category.CategoryStatsDto?> GetCategoryStatsAsync(int categoryId);
    }
}
