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

        public async Task<IEnumerable<ScoreAnomalyDto>> GetScoreAnomaliesAsync(int competitionId, double zThreshold = 2.0)
        {
            string sql = @"
                WITH ScoreStats AS (
                    SELECT
                        s.Id AS ScoreId,

                        COALESCE(
                            CONCAT(pp.Name, ' ', pp.Surname),
                            tt.Name,
                            'Unknown'
                        ) AS ParticipantName,

                        CONCAT(p.Name, ' ', p.Surname) AS JudgeName,

                        s.EntryId,

                        CASE s.Type
                            WHEN 0 THEN 'D'
                            WHEN 1 THEN 'DA'
                            WHEN 2 THEN 'DB'
                            WHEN 3 THEN 'E'
                            WHEN 4 THEN 'A'
                            WHEN 5 THEN 'Penalty'
                            ELSE CAST(s.Type AS nvarchar(50))
                        END AS ScoreType,

                        CAST(s.ScoreValue AS DECIMAL(10,2)) AS ScoreValue,

                        AVG(CAST(s.ScoreValue AS FLOAT))
                            OVER(PARTITION BY s.EntryId, s.Type)
                            AS MeanScore,

                        STDEV(CAST(s.ScoreValue AS FLOAT))
                            OVER(PARTITION BY s.EntryId, s.Type)
                            AS StdDeviation

                    FROM Scores s

                    INNER JOIN Judges j
                        ON s.JudgeId = j.Id

                    INNER JOIN Persons p
                        ON j.PersonId = p.Id

                    INNER JOIN Entries e
                        ON s.EntryId = e.Id

                    INNER JOIN Participants part
                        ON e.ParticipantId = part.Id

                    LEFT JOIN Persons pp
                        ON part.Id = pp.Id

                    LEFT JOIN Teams tt
                        ON part.Id = tt.Id

                    WHERE e.CompetitionId = {0}
                ),

                ZScores AS (
                    SELECT
                        ScoreId,
                        ParticipantName,
                        JudgeName,
                        EntryId,
                        ScoreType,
                        ScoreValue,
                        MeanScore,
                        StdDeviation,

                        CASE
                            WHEN StdDeviation = 0 THEN 0
                            ELSE
                                ABS(
                                    (ScoreValue - MeanScore)
                                    / StdDeviation
                                )
                        END AS ZScore

                    FROM ScoreStats
                )

                SELECT
                    ScoreId,
                    ParticipantName,
                    JudgeName,
                    EntryId,
                    ScoreType,
                    ScoreValue,

                    CAST(MeanScore AS DECIMAL(10,2))
                        AS AverageEntryScore,

                    CAST(ZScore AS DECIMAL(10,2))
                        AS Deviation

                FROM ZScores

                WHERE ZScore >= {1}

                ORDER BY ZScore DESC;
                ";

            return await _context.ScoreAnomalies
                .FromSqlRaw(sql, competitionId, zThreshold)
                .AsNoTracking()
                .ToListAsync();
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
