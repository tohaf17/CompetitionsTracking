using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Repositories
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public PersonRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ParticipantPerformanceDto>> GetParticipantPerformanceHistoryAsync(int participantId)
        {
            string sql = @"
                SELECT 
                    c.Id AS CompetitionId,
                    c.Title AS CompetitionName,
                    d.Type AS ApparatusName,
                    r.FinalScore,
                    r.Place AS Placement,
                    c.StartDate AS CompetitionDate
                FROM Results r
                INNER JOIN Entries e ON r.EntryId = e.Id
                INNER JOIN Competitions c ON e.CompetitionId = c.Id
                INNER JOIN Disciplines d ON e.DisciplineId = d.Id
                WHERE e.ParticipantId = {0}
                ORDER BY c.StartDate ASC
            ";
            return await _context.ParticipantPerformances.FromSqlRaw(sql, participantId).ToListAsync();
        }
        public async Task<IEnumerable<Person>> GetMenteesAsync(int mentorId)
        {
            return await _context.Persons
                .Where(p => p.MentorId == mentorId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Person?> GetPersonWithTeamsAsync(int personId)
        {
            return await _context.Persons
                .Include(p => p.TeamsCoached)
                .Include(p => p.TeamsAsMember)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == personId);
        }
    }
}
