using CompetitionsTracking.Application.DTOs.Entry;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IEntryService
    {
        Task<IEnumerable<EntryResponseDto>> GetAllAsync();
        Task<EntryResponseDto?> GetByIdAsync(int id);
        Task<EntryResponseDto> CreateAsync(EntryRequestDto request);
        Task UpdateAsync(int id, EntryRequestDto request);
        Task DeleteAsync(int id);
    }
}
