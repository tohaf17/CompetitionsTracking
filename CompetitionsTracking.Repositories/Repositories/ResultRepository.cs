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

        public async Task<IEnumerable<Result>> GetAllAsync()
        {
            return await _context.Results
                .Include(r => r.Entry).ThenInclude(e => e.Participant)
                .Include(r => r.Entry).ThenInclude(e => e.Competition)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Result?> GetByIdAsync(int id)
        {
            return await _context.Results
                .Include(r => r.Entry).ThenInclude(e => e.Participant)
                .Include(r => r.Entry).ThenInclude(e => e.Competition)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<TeamMedalTallyDto>> GetTeamMedalTallyAsync(int competitionId)
        {
            string sql = @"
                SELECT 
                    t.Id AS TeamId,
                    t.Name AS TeamName,
                    CAST(SUM(CASE WHEN m.Place = 1 THEN 1 ELSE 0 END) AS INT) AS GoldMedals,
                    CAST(SUM(CASE WHEN m.Place = 2 THEN 1 ELSE 0 END) AS INT) AS SilverMedals,
                    CAST(SUM(CASE WHEN m.Place = 3 THEN 1 ELSE 0 END) AS INT) AS BronzeMedals,
                    CAST(SUM(CASE WHEN m.Place <= 3 THEN 1 ELSE 0 END) AS INT) AS TotalMedals
                FROM teams t
                CROSS APPLY (
                    SELECT r.Place
                    FROM team_members tm
                    INNER JOIN Entries e ON tm.person_id = e.ParticipantId
                    INNER JOIN Results r ON e.Id = r.EntryId
                    WHERE tm.team_id = t.Id AND e.CompetitionId = {0} AND r.Place BETWEEN 1 AND 3
                    
                    UNION ALL
                    
                    SELECT r.Place
                    FROM Entries e
                    INNER JOIN Results r ON e.Id = r.EntryId
                    WHERE e.ParticipantId = t.Id AND e.CompetitionId = {0} AND r.Place BETWEEN 1 AND 3
                ) m
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
