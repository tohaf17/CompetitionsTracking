using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Repositories
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        public TeamRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TeamDominanceMetricDto>> GetTeamDominanceMetricsAsync(int teamId)
        {
            string sql = @"
                SELECT 
                    t.Id AS TeamId,
                    t.Name AS TeamName,
                    COUNT(DISTINCT p.Id) AS TotalParticipants,
                    SUM(r.FinalScore) AS CumulativePoints,
                    SUM(r.FinalScore) / NULLIF(COUNT(DISTINCT p.Id), 0) AS AveragePointsPerParticipant
                FROM Teams t
                INNER JOIN Participants p ON t.Id = p.TeamId
                INNER JOIN Entries e ON p.Id = e.ParticipantId
                INNER JOIN Results r ON e.Id = r.EntryId
                WHERE t.Id = {0}
                GROUP BY t.Id, t.Name
            ";
            return await _context.TeamDominanceMetrics.FromSqlRaw(sql, teamId).ToListAsync();
        }
        public async Task<Domain.Entities.Team?> GetTeamWithMembersAsync(int teamId)
        {
            return await _context.Teams
                .Include(t => t.Coach)
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == teamId);
        }
    }
}
