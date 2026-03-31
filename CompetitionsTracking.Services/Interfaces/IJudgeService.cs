using CompetitionsTracking.Application.DTOs.Judge;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IJudgeService
    {
        Task<IEnumerable<JudgeResponseDto>> GetAllAsync();
        Task<JudgeResponseDto?> GetByIdAsync(int id);
        Task<JudgeResponseDto> CreateAsync(JudgeRequestDto request);
        Task UpdateAsync(int id, JudgeRequestDto request);
        Task DeleteAsync(int id);
    }
}
