using CompetitionsTracking.Application.DTOs.Team;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamResponseDto>> GetAllAsync();
        Task<TeamResponseDto?> GetByIdAsync(int id);
        Task<TeamResponseDto> CreateAsync(TeamRequestDto request);
        Task UpdateAsync(int id, TeamRequestDto request);
        Task DeleteAsync(int id);
        Task<IEnumerable<TeamDominanceMetricDto>> GetTeamDominanceMetricsAsync(int teamId);
        Task<TeamRosterDto?> GetTeamRosterAsync(int teamId);
        Task AddMemberToTeamAsync(int teamId, int personId);
        Task RemoveMemberFromTeamAsync(int teamId, int personId);
    }
}
