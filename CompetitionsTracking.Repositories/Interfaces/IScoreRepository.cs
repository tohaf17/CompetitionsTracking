using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Interfaces
{
    public interface IScoreRepository : IRepository<Score>
    {
        Task<IEnumerable<ScoreAnomalyDto>> GetScoreAnomaliesAsync(int competitionId, double zThreshold = 2.0);
        Task<IEnumerable<Score>> GetScoresByEntryAsync(int entryId);
    }
}
