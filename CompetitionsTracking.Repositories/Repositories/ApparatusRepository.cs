using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;

namespace CompetitionsTracking.Repositories.Repositories
{
    public class ApparatusRepository : Repository<Apparatus>, IApparatusRepository
    {
        public ApparatusRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }
    }
}
