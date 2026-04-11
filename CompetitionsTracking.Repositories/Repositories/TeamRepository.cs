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
                    CAST(SUM(r.FinalScore) AS FLOAT) AS CumulativePoints,
                    CAST(SUM(r.FinalScore) / NULLIF(COUNT(DISTINCT p.Id), 0) AS FLOAT) AS AveragePointsPerParticipant
                FROM teams t
                INNER JOIN (
                    -- Individual members
                    SELECT team_id, person_id AS ParticipantId FROM team_members
                    UNION ALL
                    -- The team itself (for group entries)
                    SELECT Id, Id FROM teams
                ) m ON t.Id = m.team_id
                INNER JOIN participants p ON m.ParticipantId = p.Id
                INNER JOIN entries e ON p.Id = e.ParticipantId
                INNER JOIN results r ON e.Id = r.EntryId
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
