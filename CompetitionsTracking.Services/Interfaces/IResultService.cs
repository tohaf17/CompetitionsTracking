using CompetitionsTracking.Application.DTOs.Result;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IResultService
    {
        Task<IEnumerable<ResultResponseDto>> GetAllAsync();
        Task<ResultResponseDto?> GetByIdAsync(int id);
        Task<ResultResponseDto> CreateAsync(ResultRequestDto request);
        Task UpdateAsync(int id, ResultRequestDto request);
        Task DeleteAsync(int id);
    }
}
