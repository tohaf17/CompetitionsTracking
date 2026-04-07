using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;

namespace CompetitionsTracking.Repositories.Implementations
{
    public class CompetitionRepository : Repository<Competition>, ICompetitionRepository
    {
        public CompetitionRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }
    }
}
