using CompetitionsTracking.Application.DTOs.Appeal;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IAppealService
    {
        Task<IEnumerable<AppealResponseDto>> GetAllAsync();
        Task<AppealResponseDto?> GetByIdAsync(int id);
        Task<AppealResponseDto> CreateAsync(AppealRequestDto request);
        Task UpdateAsync(int id, AppealRequestDto request);
        Task DeleteAsync(int id);
        Task ApproveAppealAsync(int id, ApproveAppealRequestDto request);
        Task<IEnumerable<PendingAppealDto>> GetPendingAppealsAsync(int? competitionId);
        Task<AppealDossierDto?> GetAppealDossierAsync(int id);
    }
}
