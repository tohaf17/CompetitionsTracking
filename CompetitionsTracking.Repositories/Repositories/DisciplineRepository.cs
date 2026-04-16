using CompetitionsTracking.Application.DTOs.Discipline;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsTracking.Repositories.Repositories
{
    public class DisciplineRepository : Repository<Discipline>, IDisciplineRepository
    {
        public DisciplineRepository(CompetitionsTrackingDbContext context) : base(context)
        {

        }
        public async Task<DisciplineStatsDto?> GetDisciplineStatsAsync(int disciplineId)
        {
            var stats = await _context.Database.SqlQuery<DisciplineStatsDto>($@"
                SELECT 
                    d.Id AS DisciplineId,
                    d.Type AS DisciplineName,
                    CAST(COUNT(DISTINCT e.Id) AS INT) AS TotalEntries,
                    CAST(COUNT(DISTINCT e.CompetitionId) AS INT) AS CompetitionsFeaturedIn,
                    CAST(ISNULL(ROUND(AVG(CAST(s.ScoreValue AS FLOAT)), 2), 0) AS REAL) AS AverageScore
                FROM Disciplines d
                LEFT JOIN Entries e ON d.Id = e.DisciplineId
                LEFT JOIN Scores s ON e.Id = s.EntryId
                WHERE d.Id = {disciplineId}
                GROUP BY d.Id, d.Type"
            ).FirstOrDefaultAsync();

            return stats;
        }
    }
}
