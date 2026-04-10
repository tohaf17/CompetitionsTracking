using CompetitionsTracking.Application.DTOs.Score;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IScoreService
    {
        Task<IEnumerable<ScoreResponseDto>> GetAllAsync();
        Task<ScoreResponseDto?> GetByIdAsync(int id);
        Task<ScoreResponseDto> CreateAsync(ScoreRequestDto request);
        Task UpdateAsync(int id, ScoreRequestDto request);
        Task DeleteAsync(int id);
        Task<IEnumerable<ScoreAnomalyDto>> GetScoreAnomaliesAsync(int competitionId);
        Task<IEnumerable<EntryScoreDetailDto>> GetScoresByEntryAsync(int entryId);
        Task<EntryScoreBreakdownDto?> GetEntryScoreBreakdownAsync(int entryId);
    }
}
