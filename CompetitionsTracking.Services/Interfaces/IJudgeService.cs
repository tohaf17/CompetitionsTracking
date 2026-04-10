using CompetitionsTracking.Application.DTOs.Judge;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IJudgeService
    {
        Task<IEnumerable<JudgeResponseDto>> GetAllAsync();
        Task<JudgeResponseDto?> GetByIdAsync(int id);
        Task<JudgeResponseDto> CreateAsync(JudgeRequestDto request);
        Task UpdateAsync(int id, JudgeRequestDto request);
        Task DeleteAsync(int id);
        Task<IEnumerable<JudgeAnalyticsDto>> GetJudgeAnalyticsAsync(int judgeId);
        Task<IEnumerable<PendingEvaluationDto>> GetPendingEvaluationsAsync(int judgeId, int competitionId);
        Task<IEnumerable<ConflictOfInterestDto>> GetConflictsOfInterestAsync(int judgeId);
        Task<IEnumerable<JudgeWorkloadDto>> GetWorkloadSummaryAsync(int judgeId, int competitionId);
        Task<IEnumerable<JudgeScoreHistoryDto>> GetJudgeScoresInCompetitionAsync(int judgeId, int competitionId);
    }
}
