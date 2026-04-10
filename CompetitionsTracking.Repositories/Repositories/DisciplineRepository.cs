using CompetitionsTracking.Application.DTOs.Discipline;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsTracking.Repositories.Repositories
{
    public class DisciplineRepository : Repository<Discipline>, IDisciplineRepository
    {
        public DisciplineRepository(CompetitionsTrackingDbContext context) : base(context)
        {

        }
        public async Task<DisciplineStatsDto?> GetDisciplineStatsAsync(int disciplineId)
        {
            var discipline = await _context.Disciplines.FindAsync(disciplineId);
            if (discipline == null) return null;

            // Скільки всього заявок подано на цю дисципліну
            var totalEntries = await _context.Entries
                .CountAsync(e => e.DisciplineId == disciplineId);

            // У скількох унікальних змаганнях фігурувала ця дисципліна
            var competitionsCount = await _context.Entries
                .Where(e => e.DisciplineId == disciplineId)
                .Select(e => e.CompetitionId)
                .Distinct()
                .CountAsync();

            // Загальний середній бал за всі часи (беремо всі оцінки, де заявка має цю дисципліну)
            var avgScore = await _context.Scores
                .Include(s => s.Entry)
                .Where(s => s.Entry.DisciplineId == disciplineId)
                .AverageAsync(s => (float?)s.ScoreValue) ?? 0f;

            return new DisciplineStatsDto
            {
                DisciplineId = disciplineId,
                DisciplineName = discipline.Type, // Припускаємо, що поле називається Name
                TotalEntries = totalEntries,
                CompetitionsFeaturedIn = competitionsCount,
                AverageScore = (float)System.Math.Round(avgScore, 2)
            };
        }
    }
}
