using CompetitionsTracking.Application.DTOs.Score;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IScoreService
    {
        Task<IEnumerable<ScoreResponseDto>> GetAllAsync();
        Task<ScoreResponseDto?> GetByIdAsync(int id);
        Task<ScoreResponseDto> CreateAsync(ScoreRequestDto request);
        Task UpdateAsync(int id, ScoreRequestDto request);
        Task DeleteAsync(int id);
    }
}
