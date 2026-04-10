using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Interfaces
{
    public interface ITeamRepository : IRepository<Team>
    {
        Task<IEnumerable<TeamDominanceMetricDto>> GetTeamDominanceMetricsAsync(int teamId);
        Task<Domain.Entities.Team?> GetTeamWithMembersAsync(int teamId);
    }
}
