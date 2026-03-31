using CompetitionsTracking.Application.DTOs.Team;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamResponseDto>> GetAllAsync();
        Task<TeamResponseDto?> GetByIdAsync(int id);
        Task<TeamResponseDto> CreateAsync(TeamRequestDto request);
        Task UpdateAsync(int id, TeamRequestDto request);
        Task DeleteAsync(int id);
    }
}
