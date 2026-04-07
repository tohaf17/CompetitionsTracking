using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Repositories.Interfaces
{
    public interface IAppealRepository : IRepository<Appeal>
    {
        Task ApproveAppealWithRecalculationAsync(int appealId, int scoreId, float newScoreValue);
        Task<bool> HasAppealForResultAsync(int resultId);
        Task<bool> IsCompetitionOngoingForResultAsync(int resultId);
        Task<IEnumerable<Appeal>> GetPendingAppealsAsync(int? competitionId);
        Task<Appeal?> GetAppealDossierAsync(int id);
    }
}
