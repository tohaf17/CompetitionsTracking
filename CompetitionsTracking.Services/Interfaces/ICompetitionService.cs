using CompetitionsTracking.Application.DTOs.Common;
using CompetitionsTracking.Application.DTOs.Competition;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface ICompetitionService
    {
        Task<CompetitionResponseDto?> GetByIdAsync(int id);
        Task<CompetitionResponseDto> CreateAsync(CompetitionRequestDto request);
        Task UpdateAsync(int id, CompetitionRequestDto request);
        Task DeleteAsync(int id);
        Task<IEnumerable<LeaderboardDto>> GetCompetitionLeaderboardAsync(int competitionId);
        Task<PagedResponse<CompetitionResponseDto>> GetAllAsync(CompetitionFilterDto? filter = null, PaginationParams? pagination = null);

        Task ChangeStatusAsync(int id, ChangeCompetitionStatusDto request);
        Task AwardMedalsAsync(int id);
        Task<CompetitionSummaryDto?> GetSummaryAsync(int id);
    }
}
