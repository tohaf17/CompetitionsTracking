using CompetitionsTracking.Application.DTOs.Discipline;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IDisciplineService
    {
        Task<IEnumerable<DisciplineResponseDto>> GetAllAsync();
        Task<DisciplineResponseDto?> GetByIdAsync(int id);
        Task<DisciplineResponseDto> CreateAsync(DisciplineRequestDto request);
        Task UpdateAsync(int id, DisciplineRequestDto request);
        Task DeleteAsync(int id);
        Task<DisciplineStatsDto?> GetDisciplineStatsAsync(int disciplineId);
    }
}
