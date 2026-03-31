using CompetitionsTracking.Application.DTOs.Competition;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface ICompetitionService
    {
        Task<IEnumerable<CompetitionResponseDto>> GetAllAsync();
        Task<CompetitionResponseDto?> GetByIdAsync(int id);
        Task<CompetitionResponseDto> CreateAsync(CompetitionRequestDto request);
        Task UpdateAsync(int id, CompetitionRequestDto request);
        Task DeleteAsync(int id);
    }
}
