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
                        COALESCE(CONCAT(pp.Name, ' ', pp.Surname), tt.Name, 'Unknown') AS ParticipantName,
                        CONCAT(p.Name, ' ', p.Surname) AS JudgeName,
                        s.EntryId,
                        CAST(s.Type AS nvarchar(50)) AS ScoreType,
                        CAST(s.ScoreValue AS REAL) AS ScoreValue,
                        CAST(AVG(CAST(s.ScoreValue AS float)) OVER(PARTITION BY s.EntryId) AS REAL) AS AverageEntryScore
                    FROM Scores s
                    INNER JOIN Judges j ON s.JudgeId = j.Id
                    INNER JOIN Persons p ON j.PersonId = p.Id
                    INNER JOIN Entries e ON s.EntryId = e.Id
                    INNER JOIN Participants part ON e.ParticipantId = part.Id
                    LEFT JOIN Persons pp ON part.Id = pp.Id
                    LEFT JOIN Teams tt ON part.Id = tt.Id
                    WHERE e.CompetitionId = {0}
                )
                SELECT 
                    ScoreId,
                    ParticipantName,
                    JudgeName,
                    EntryId,
                    ScoreType,
                    ScoreValue,
                    AverageEntryScore,
                    CAST(ABS(ScoreValue - AverageEntryScore) AS REAL) AS Deviation
                FROM ScoreAverages
                WHERE ABS(ScoreValue - AverageEntryScore) >= 1.5
                ORDER BY Deviation DESC
            ";
            return await _context.ScoreAnomalies.FromSqlRaw(sql, competitionId).ToListAsync();
        }
        public async Task<IEnumerable<Score>> GetScoresByEntryAsync(int entryId)
        {
            return await _context.Scores
                .Include(s => s.Judge).ThenInclude(j => j.Person)
                .Where(s => s.EntryId == entryId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
