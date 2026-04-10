using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Application.DTOs.Competition;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Interfaces
{
    public interface ICompetitionRepository : IRepository<Competition>
    {
        Task<IEnumerable<LeaderboardDto>> GetCompetitionLeaderboardAsync(int competitionId);
        Task<IEnumerable<Competition>> GetFilteredAsync(CompetitionFilterDto filter);
        Task<CompetitionSummaryDto?> GetSummaryAsync(int competitionId);
        Task AwardMedalsAsync(int competitionId);
    }
}
