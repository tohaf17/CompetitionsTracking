using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Interfaces
{
    public interface IJudgeRepository : IRepository<Judge>
    {
        Task<IEnumerable<JudgeAnalyticsDto>> GetJudgeAnalyticsAsync(int judgeId);
        Task<IEnumerable<Domain.Entities.Entry>> GetPendingEvaluationsAsync(int judgeId, int competitionId);
        Task<IEnumerable<Domain.Entities.Score>> GetConflictsOfInterestAsync(int judgeId);
        Task<IEnumerable<dynamic>> GetWorkloadSummaryAsync(int judgeId, int competitionId);
        Task<IEnumerable<Domain.Entities.Score>> GetJudgeScoresInCompetitionAsync(int judgeId, int competitionId);
    }
}
