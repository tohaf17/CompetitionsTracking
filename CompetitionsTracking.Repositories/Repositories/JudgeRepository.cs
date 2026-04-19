using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Repositories
{
    public class JudgeRepository : Repository<Judge>, IJudgeRepository
    {
        public JudgeRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<JudgeAnalyticsDto>> GetJudgeAnalyticsAsync(int judgeId)
        {
            string sql = @"
                WITH EntryAverages AS (
                    SELECT EntryId, AVG(ScoreValue) AS AvgTotalScore
                    FROM Scores
                    GROUP BY EntryId
                )
                SELECT 
                    j.Id AS JudgeId,
                    CONCAT(p.Name, ' ', p.Surname) AS JudgeName,
                    COUNT(s.Id) AS TotalPerformancesJudged,
                    AVG(s.ScoreValue) AS AverageScoreGiven,
                    ROUND(AVG(s.ScoreValue - ea.AvgTotalScore), 4) AS AverageScoreDeviation
                FROM Judges j
                INNER JOIN Persons p ON j.PersonId = p.Id
                INNER JOIN Scores s ON j.Id = s.JudgeId
                INNER JOIN EntryAverages ea ON s.EntryId = ea.EntryId
                WHERE j.Id = {0}
                GROUP BY j.Id, p.Name, p.Surname
            ";

            return await _context.JudgeAnalytics.FromSqlRaw(sql, judgeId).ToListAsync();
        }
        public async Task<IEnumerable<Entry>> GetPendingEvaluationsAsync(int judgeId, int competitionId)
        {
            return await _context.Entries
                .Include(e => e.Participant) 
                .Include(e => e.Discipline)
                .Include(e => e.Category)
                .Where(e => e.CompetitionId == competitionId
                         && e.ApplicationStatus == ApplicationStatus.Accepted
                         && e.EntryStatus != EntryStatus.DNS
                         && !e.Scores.Any(s => s.JudgeId == judgeId))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Score>> GetConflictsOfInterestAsync(int judgeId)
        {
            return await _context.Scores
                .Include(s => s.Judge).ThenInclude(j => j.Person)
                .Include(s => s.Entry).ThenInclude(e => e.Participant)
                .Where(s => s.JudgeId == judgeId
                         && s.Entry.Participant is Person
                         && ((Person)s.Entry.Participant).Country == s.Judge.Person.Country)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkloadSummaryDto>> GetWorkloadSummaryAsync(int judgeId, int competitionId)
        {
            return await _context.Scores
                .Include(s => s.Entry).ThenInclude(e => e.Discipline)
                .Where(s => s.JudgeId == judgeId && s.Entry.CompetitionId == competitionId)
                .GroupBy(s => s.Entry.Discipline.Type)
                .Select(g => new WorkloadSummaryDto(g.Key, g.Count()))
                .ToListAsync();
        }

        public async Task<IEnumerable<Score>> GetJudgeScoresInCompetitionAsync(int judgeId, int competitionId)
        {
            return await _context.Scores
                .Include(s => s.Entry).ThenInclude(e => e.Participant)
                .Where(s => s.JudgeId == judgeId && s.Entry.CompetitionId == competitionId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
