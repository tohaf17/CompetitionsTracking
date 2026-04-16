using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Interfaces
{
    public interface IJudgeRepository : IRepository<Judge>
    {
        Task<IEnumerable<JudgeAnalyticsDto>> GetJudgeAnalyticsAsync(int judgeId);
        Task<IEnumerable<Entry>> GetPendingEvaluationsAsync(int judgeId, int competitionId);
        Task<IEnumerable<Score>> GetConflictsOfInterestAsync(int judgeId);
        Task<IEnumerable<WorkloadSummaryDto>> GetWorkloadSummaryAsync(int judgeId, int competitionId);
        Task<IEnumerable<Score>> GetJudgeScoresInCompetitionAsync(int judgeId, int competitionId);
    }
}
