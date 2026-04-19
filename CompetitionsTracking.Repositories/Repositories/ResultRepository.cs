using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Repositories
{
    public class ResultRepository : Repository<Result>, IResultRepository
    {
        public ResultRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TeamMedalTallyDto>> GetTeamMedalTallyAsync(int competitionId)
        {
            string sql = @"
                SELECT 
                    t.Id AS TeamId,
                    t.Name AS TeamName,
                    SUM(CASE WHEN r.Place = 1 THEN 1 ELSE 0 END) AS GoldMedals,
                    SUM(CASE WHEN r.Place = 2 THEN 1 ELSE 0 END) AS SilverMedals,
                    SUM(CASE WHEN r.Place = 3 THEN 1 ELSE 0 END) AS BronzeMedals,
                    SUM(CASE WHEN r.Place <= 3 THEN 1 ELSE 0 END) AS TotalMedals
                FROM Results r
                INNER JOIN Entries e ON r.EntryId = e.Id
                INNER JOIN Teams t ON e.ParticipantId = t.Id
                WHERE e.CompetitionId = {0} AND r.Place <= 3
                GROUP BY t.Id, t.Name
                ORDER BY TotalMedals DESC, GoldMedals DESC
            ";
            return await _context.TeamMedalTallies.FromSqlRaw(sql, competitionId).ToListAsync();
        }
        public async Task<IEnumerable<Result>> GetLeaderboardAsync(int competitionId, int? disciplineId, int? categoryId)
        {
            var query = _context.Results
                .Include(r => r.Entry).ThenInclude(e => e.Participant)
                .Include(r => r.Entry).ThenInclude(e => e.Discipline)
                .Include(r => r.Entry).ThenInclude(e => e.Category)
                .Where(r => r.Entry.CompetitionId == competitionId);

            if (disciplineId.HasValue)
            {
                query = query.Where(r => r.Entry.DisciplineId == disciplineId.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(r => r.Entry.CategoryId == categoryId.Value);
            }

            return await query
                .OrderBy(r => r.Place) 
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Result>> GetMedalistsByCompetitionAsync(int competitionId)
        {
            return await _context.Results
                .Include(r => r.Entry).ThenInclude(e => e.Participant)
                .Where(r => r.Entry.CompetitionId == competitionId && r.Place >= 1 && r.Place <= 3)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Result>> GetTopRecordsByDisciplineAsync(int disciplineId, int topN)
        {
            return await _context.Results
                .Include(r => r.Entry).ThenInclude(e => e.Participant)
                .Include(r => r.Entry).ThenInclude(e => e.Competition)
                .Where(r => r.Entry.DisciplineId == disciplineId)
                .OrderByDescending(r => r.FinalScore) 
                .Take(topN)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
