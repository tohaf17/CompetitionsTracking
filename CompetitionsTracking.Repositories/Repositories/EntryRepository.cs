using CompetitionsTracking.Application.DTOs.Entry;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Repositories
{
    public class EntryRepository : Repository<Entry>, IEntryRepository
    {
        public EntryRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ControversialEntryDto>> GetControversialEntriesAsync(int competitionId)
        {
            string sql = @"
                SELECT 
                    e.Id AS EntryId,
                    CONCAT(p.FirstName, ' ', p.LastName) AS ParticipantName,
                    c.Name AS CompetitionName,
                    MAX(s.ScoreValue) AS HighestScore,
                    MIN(s.ScoreValue) AS LowestScore,
                    MAX(s.ScoreValue) - MIN(s.ScoreValue) AS ScoreGap
                FROM Entries e
                INNER JOIN Scores s ON e.Id = s.EntryId
                INNER JOIN Participants part ON e.ParticipantId = part.Id
                INNER JOIN Persons p ON part.Id = p.Id
                INNER JOIN Competitions c ON e.CompetitionId = c.Id
                WHERE e.CompetitionId = {0}
                GROUP BY e.Id, p.FirstName, p.LastName, c.Name
                ORDER BY ScoreGap DESC
            ";
            return await _context.ControversialEntries.FromSqlRaw(sql, competitionId).ToListAsync();
        }
        public async Task<int> BulkUpdateAppStatusAsync(int competitionId, int categoryId, ApplicationStatus newStatus)
        {
            return await _context.Entries
                .Where(e => e.CompetitionId == competitionId
                         && e.CategoryId == categoryId
                         && e.ApplicationStatus == ApplicationStatus.Pending)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.ApplicationStatus, newStatus));
        }

        public async Task<IEnumerable<Entry>> GetStartListAsync(int competitionId)
        {
            return await _context.Entries
                .Include(e => e.Participant)
                .Include(e => e.Category)
                .Include(e => e.Discipline)
                .Where(e => e.CompetitionId == competitionId && e.ApplicationStatus == ApplicationStatus.Accepted)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<IEnumerable<Entry>> GetEntriesWithMissingScoresAsync(int competitionId, int expectedScoresCount)
        {
            return await _context.Entries
                .Include(e => e.Participant)
                .Include(e => e.Scores)
                .Where(e => e.CompetitionId == competitionId
                         && e.EntryStatus == EntryStatus.Finished
                         && e.Scores.Count < expectedScoresCount)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<EntryAnalyticsDto> GetEntryAnalyticsAsync(int competitionId)
        {
            var total = await _context.Entries.CountAsync(e => e.CompetitionId == competitionId);

            var statuses = await _context.Entries
                .Where(e => e.CompetitionId == competitionId)
                .GroupBy(e => e.ApplicationStatus)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToDictionaryAsync(k => k.Status, v => v.Count);

            var categories = await _context.Entries
                .Include(e => e.Category)
                .Where(e => e.CompetitionId == competitionId)
                .GroupBy(e => e.Category.Type ?? "Unknown")
                .Select(g => new { CategoryName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(k => k.CategoryName, v => v.Count);

            return new EntryAnalyticsDto
            {
                TotalEntries = total,
                EntriesByStatus = statuses,
                EntriesByCategory = categories
            };
        }

        public async Task<bool> IsDuplicateEntryAsync(int competitionId, int participantId, int disciplineId)
        {
            return await _context.Entries.AnyAsync(e =>
                e.CompetitionId == competitionId &&
                e.ParticipantId == participantId &&
                e.DisciplineId == disciplineId);
        }

        public async Task<Entry?> GetEntryWithResultAsync(int id)
        {
            return await _context.Entries
                .Include(e => e.Result)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
