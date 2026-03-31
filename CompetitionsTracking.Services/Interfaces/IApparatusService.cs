using CompetitionsTracking.Application.DTOs.Apparatus;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IApparatusService
    {
        Task<IEnumerable<ApparatusResponseDto>> GetAllAsync();
        Task<ApparatusResponseDto?> GetByIdAsync(int id);
        Task<ApparatusResponseDto> CreateAsync(ApparatusRequestDto request);
        Task UpdateAsync(int id, ApparatusRequestDto request);
        Task DeleteAsync(int id);
    }
}
