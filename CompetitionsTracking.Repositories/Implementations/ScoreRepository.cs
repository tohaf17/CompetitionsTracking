using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;

namespace CompetitionsTracking.Repositories.Implementations
{
    public class ScoreRepository : Repository<Score>, IScoreRepository
    {
        public ScoreRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }
    }
}
