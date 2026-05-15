using CompetitionsTracking.Application.DTOs.Appeal;
using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IAppealService
    {
        Task<IEnumerable<AppealResponseDto>> GetAllAsync();
        Task<IEnumerable<AppealResponseDto>> GetAllForUserAsync(int userId, UserRole role);
        Task<AppealResponseDto?> GetByIdAsync(int id);
        Task<AppealResponseDto> CreateAsync(AppealRequestDto request);
        Task<AppealResponseDto> CreateAsync(AppealRequestDto request, int userId, UserRole role);
        Task UpdateAsync(int id, AppealRequestDto request);
        Task DeleteAsync(int id);
        Task ApproveAppealAsync(int id, ApproveAppealRequestDto request);
        Task<IEnumerable<PendingAppealDto>> GetPendingAppealsAsync(int? competitionId);
        Task<IEnumerable<PendingAppealDto>> GetPendingAppealsForUserAsync(int? competitionId, int userId, UserRole role);
        Task<AppealDossierDto?> GetAppealDossierAsync(int id);
        Task<AppealDossierDto?> GetAppealDossierAsync(int id, int userId, UserRole role);
    }
}
