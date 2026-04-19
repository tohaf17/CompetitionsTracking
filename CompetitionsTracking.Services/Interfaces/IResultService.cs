using CompetitionsTracking.Application.DTOs.Result;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IResultService
    {
        Task<IEnumerable<ResultResponseDto>> GetAllAsync();
        Task<ResultResponseDto?> GetByIdAsync(int id);
        Task<ResultResponseDto> CreateAsync(ResultRequestDto request);
        Task UpdateAsync(int id, ResultRequestDto request);
        Task DeleteAsync(int id);
        Task<IEnumerable<TeamMedalTallyDto>> GetTeamMedalTallyAsync(int competitionId);
        Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync(int competitionId, int? disciplineId, int? categoryId);
        Task<IEnumerable<CountryMedalTallyDto>> GetCountryMedalTallyAsync(int competitionId);
        Task<IEnumerable<DisciplineRecordDto>> GetTopRecordsByDisciplineAsync(int disciplineId, int topN = 10);
    }
}
