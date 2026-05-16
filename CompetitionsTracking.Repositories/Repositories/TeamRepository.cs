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
                    (SELECT COUNT(*) FROM team_members WHERE team_id = t.Id) AS TotalParticipants,
                    CAST(ISNULL(m.Points, 0) AS FLOAT) AS CumulativePoints,
                    CAST(ISNULL(m.Points, 0) / NULLIF((SELECT COUNT(*) FROM team_members WHERE team_id = t.Id), 0) AS FLOAT) AS AveragePointsPerParticipant
                FROM teams t
                LEFT JOIN (
                    SELECT m.team_id, SUM(r.FinalScore) AS Points
                    FROM (
                        SELECT team_id, person_id AS ParticipantId FROM team_members
                        UNION ALL
                        SELECT Id, Id FROM teams
                    ) m
                    INNER JOIN entries e ON m.ParticipantId = e.ParticipantId
                    INNER JOIN results r ON e.Id = r.EntryId
                    GROUP BY m.team_id
                ) m ON t.Id = m.team_id
                WHERE t.Id = {0}
            ";
            return await _context.TeamDominanceMetrics.FromSqlRaw(sql, teamId).ToListAsync();
        }
        public async Task<Team?> GetTeamWithMembersAsync(int teamId)
        {
            return await _context.Teams
                .Include(t => t.Coach)
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == teamId);
        }

        public async Task<IEnumerable<Team>> GetAllWithCoachAsync()
        {
            return await _context.Teams
                .Include(t => t.Coach)
                .ToListAsync();
        }
        public async Task<IEnumerable<Team>> GetAllForCoachAsync(int coachPersonId)
        {
            return await _context.Teams
                .Include(t => t.Coach)
                .Include(t => t.Members)
                .Where(t => t.CoachId == coachPersonId)
                .ToListAsync();
        }
    }
}
