using CompetitionsTracking.Application.DTOs.Category;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsTracking.Repositories.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }
        public async Task<CategoryStatsDto?> GetCategoryStatsAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null) return null;

            // Кількість всіх заявок у цій категорії
            var totalEntries = await _context.Entries
                .CountAsync(e => e.CategoryId == categoryId);

            // Кількість унікальних змагань, де була ця категорія
            var competitionsCount = await _context.Entries
                .Where(e => e.CategoryId == categoryId)
                .Select(e => e.CompetitionId)
                .Distinct()
                .CountAsync();

            // Середній бал у цій категорії за всі часи
            var avgScore = await _context.Scores
                .Include(s => s.Entry)
                .Where(s => s.Entry.CategoryId == categoryId)
                .AverageAsync(s => (float?)s.ScoreValue) ?? 0f;

            return new CategoryStatsDto
            {
                CategoryId = categoryId,
                CategoryType = category.Type, // Переконайся, що у сутності Category є поле Type
                TotalEntries = totalEntries,
                CompetitionsFeaturedIn = competitionsCount,
                AverageScore = (float)System.Math.Round(avgScore, 2)
            };
        }
    }
}
