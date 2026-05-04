using CompetitionsTracking.Application.DTOs.Competition;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Repositories
{
    public class CompetitionRepository : Repository<Competition>, ICompetitionRepository
    {
        public CompetitionRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LeaderboardDto>> GetCompetitionLeaderboardAsync(int competitionId)
        {
            string sql = @"
                SELECT 
                    p.Id AS ParticipantId,
                    CONCAT(p.Name, ' ', p.Surname) AS ParticipantName,
                    c.Type AS CategoryName,
                    d.Type AS DisciplineName,
                    r.FinalScore AS TotalScore,
                    DENSE_RANK() OVER(PARTITION BY e.CategoryId, e.DisciplineId ORDER BY r.FinalScore DESC) AS CalculatedRank
                FROM results r
                INNER JOIN entries e ON r.EntryId = e.Id
                INNER JOIN participants part ON e.ParticipantId = part.Id
                INNER JOIN persons p ON part.Id = p.Id
                INNER JOIN categories c ON e.CategoryId = c.Id
                INNER JOIN disciplines d ON e.DisciplineId = d.Id
                WHERE e.CompetitionId = {0}
            ";

            return await _context.Leaderboards.FromSqlRaw(sql, competitionId).ToListAsync();
        }

        public async Task<IEnumerable<Competition>> GetFilteredAsync(CompetitionFilterDto filter)
        {
            var query = _context.Set<Competition>().AsQueryable().AsNoTracking();

            if (filter.Status.HasValue)
                query = query.Where(c => c.Status == filter.Status.Value);

            if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(c => c.City.Contains(filter.City));

            return await query.ToListAsync();
        }

        public async Task<CompetitionSummaryDto?> GetSummaryAsync(int competitionId)
        {
            string pendingStatus = ApplicationStatus.Pending.ToString();
            string acceptedStatus = ApplicationStatus.Accepted.ToString();

            var summary = await _context.Database.SqlQuery<CompetitionSummaryDto>($@"
        SELECT 
            c.Id AS CompetitionId,
            CAST(COUNT(e.Id) AS INT) AS TotalEntries,
            CAST(SUM(CASE WHEN e.ApplicationStatus = {pendingStatus} THEN 1 ELSE 0 END) AS INT) AS PendingEntries,
            CAST(SUM(CASE WHEN e.ApplicationStatus = {acceptedStatus} THEN 1 ELSE 0 END) AS INT) AS AcceptedEntries,
            CAST(COUNT(DISTINCT e.DisciplineId) AS INT) AS UniqueDisciplinesCount
        FROM competitions c
        LEFT JOIN entries e ON c.Id = e.CompetitionId
        WHERE c.Id = {competitionId}
        GROUP BY c.Id"
            ).FirstOrDefaultAsync();

            return summary;
        }

        public async Task AwardMedalsAsync(int competitionId)
        {
            string sql = @"
        WITH RankedResults AS (
            SELECT r.Id, 
                   DENSE_RANK() OVER(PARTITION BY e.CategoryId, e.DisciplineId ORDER BY r.FinalScore DESC) as Rnk
            FROM results r
            INNER JOIN entries e ON r.EntryId = e.Id
            WHERE e.CompetitionId = {0}
        )
        UPDATE r
        SET r.AwardedMedal = CASE 
            WHEN rr.Rnk = 1 THEN 'gold'
            WHEN rr.Rnk = 2 THEN 'silver'
            WHEN rr.Rnk = 3 THEN 'bronze'
            ELSE NULL
        END,
        r.Place = rr.Rnk
        FROM results r
        INNER JOIN RankedResults rr ON r.Id = rr.Id;
    ";

            await _context.Database.ExecuteSqlRawAsync(sql, competitionId);
        }
    }
}
