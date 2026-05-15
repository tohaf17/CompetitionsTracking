using CompetitionsTracking.Application.DTOs.Common;
using CompetitionsTracking.Application.DTOs.Entry;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IEntryService
    {
        Task<PagedResponse<EntryResponseDto>> GetAllAsync(PaginationParams? pagination = null);
        Task<PagedResponse<EntryResponseDto>> GetAllForUserAsync(PaginationParams? pagination, int userId, UserRole role);
        Task<EntryResponseDto?> GetByIdAsync(int id);
        Task<EntryResponseDto> CreateAsync(EntryRequestDto request);
        Task<EntryResponseDto> CreateAsync(EntryRequestDto request, int userId, UserRole role);
        Task UpdateAsync(int id, EntryRequestDto request);
        Task DeleteAsync(int id);
        Task<IEnumerable<ControversialEntryDto>> GetControversialEntriesAsync(int competitionId);
        Task<int> BulkUpdateAppStatusAsync(BulkUpdateAppStatusDto request);
        Task ChangeApplicationStatusAsync(int id, ChangeApplicationStatusDto request);
        Task ChangeEntryStatusAsync(int id, ChangeEntryStatusDto request);
        Task DisqualifyAsync(int entryId);
        Task TransferEntryAsync(int entryId, TransferEntryDto request);
        Task<IEnumerable<EntryResponseDto>> GetStartListAsync(int competitionId);
        Task<IEnumerable<EntryResponseDto>> GetMissingScoresAsync(int competitionId, int expectedCount);
        Task<EntryAnalyticsDto> GetAnalyticsAsync(int competitionId);
        Task<IEnumerable<EntryResponseDto>> GetByCompetitionIdAsync(int competitionId);
        Task<IEnumerable<EntryResponseDto>> GetByCompetitionIdForUserAsync(int competitionId, int userId, UserRole role);
        Task<IEnumerable<EntryParticipantOptionDto>> GetParticipantOptionsForUserAsync(int userId, UserRole role);
    }
}
