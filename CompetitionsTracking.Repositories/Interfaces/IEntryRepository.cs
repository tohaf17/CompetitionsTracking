using CompetitionsTracking.Application.DTOs.Entry;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Interfaces
{
    public interface IEntryRepository : IRepository<Entry>
    {
        Task<IEnumerable<ControversialEntryDto>> GetControversialEntriesAsync(int competitionId);
        Task<int> BulkUpdateAppStatusAsync(int competitionId, int categoryId, ApplicationStatus newStatus);
        Task<IEnumerable<Entry>> GetStartListAsync(int competitionId);
        Task<IEnumerable<Entry>> GetEntriesWithMissingScoresAsync(int competitionId, int expectedScoresCount);
        Task<EntryAnalyticsDto> GetEntryAnalyticsAsync(int competitionId);
        Task<bool> IsDuplicateEntryAsync(int competitionId, int participantId, int disciplineId);
        Task<Entry?> GetEntryWithResultAsync(int id);
    }
}
