using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;

namespace CompetitionsTracking.Repositories.Implementations
{
    public class AppealRepository : Repository<Appeal>, IAppealRepository
    {
        public AppealRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }
    }
}
