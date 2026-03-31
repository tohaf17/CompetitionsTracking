using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;

namespace CompetitionsTracking.Repositories.Implementations
{
    public class ApparatusRepository : Repository<Apparatus>, IApparatusRepository
    {
        public ApparatusRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }
    }
}
