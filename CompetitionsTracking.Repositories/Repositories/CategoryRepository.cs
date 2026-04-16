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
            var stats = await _context.Database.SqlQuery<CategoryStatsDto>($@"
                SELECT 
                    c.Id AS CategoryId,
                    c.Type AS CategoryType,
                    CAST(COUNT(DISTINCT e.Id) AS INT) AS TotalEntries,
                    CAST(COUNT(DISTINCT e.CompetitionId) AS INT) AS CompetitionsFeaturedIn,
                    CAST(ISNULL(ROUND(AVG(CAST(s.ScoreValue AS FLOAT)), 2), 0) AS REAL) AS AverageScore
                FROM Categories c
                LEFT JOIN Entries e ON c.Id = e.CategoryId
                LEFT JOIN Scores s ON e.Id = s.EntryId
                WHERE c.Id = {categoryId}
                GROUP BY c.Id, c.Type"
            ).FirstOrDefaultAsync();

            return stats;
        }
    }
}
