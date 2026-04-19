using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Interfaces
{
    public interface IResultRepository : IRepository<Result>
    {
        Task<IEnumerable<TeamMedalTallyDto>> GetTeamMedalTallyAsync(int competitionId);
        Task<IEnumerable<Result>> GetLeaderboardAsync(int competitionId, int? disciplineId, int? categoryId);
        Task<IEnumerable<Result>> GetMedalistsByCompetitionAsync(int competitionId);
        Task<IEnumerable<Result>> GetTopRecordsByDisciplineAsync(int disciplineId, int topN);
    }
}
