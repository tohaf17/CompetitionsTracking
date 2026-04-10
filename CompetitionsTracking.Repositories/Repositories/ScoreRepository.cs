using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Repositories
{
    public class ScoreRepository : Repository<Score>, IScoreRepository
    {
        public ScoreRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ScoreAnomalyDto>> GetScoreAnomaliesAsync(int competitionId)
        {
            string sql = @"
                WITH ScoreAverages AS (
                    SELECT 
                        s.Id AS ScoreId,
                        CONCAT(p.FirstName, ' ', p.LastName) AS JudgeName,
                        s.EntryId,
                        s.ScoreValue,
                        AVG(s.ScoreValue) OVER(PARTITION BY s.EntryId) AS AverageEntryScore
                    FROM Scores s
                    INNER JOIN Judges j ON s.JudgeId = j.Id
                    INNER JOIN Persons p ON j.Id = p.Id
                    INNER JOIN Entries e ON s.EntryId = e.Id
                    WHERE e.CompetitionId = {0}
                )
                SELECT 
                    ScoreId,
                    JudgeName,
                    EntryId,
                    ScoreValue,
                    AverageEntryScore,
                    ABS(ScoreValue - AverageEntryScore) AS Deviation
                FROM ScoreAverages
                WHERE ABS(ScoreValue - AverageEntryScore) >= 1.5
                ORDER BY Deviation DESC
            ";
            return await _context.ScoreAnomalies.FromSqlRaw(sql, competitionId).ToListAsync();
        }
        public async Task<IEnumerable<Domain.Entities.Score>> GetScoresByEntryAsync(int entryId)
        {
            return await _context.Scores
                .Include(s => s.Judge).ThenInclude(j => j.Person)
                .Where(s => s.EntryId == entryId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
