using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;

namespace CompetitionsTracking.Repositories.Implementations
{
    public class ResultRepository : Repository<Result>, IResultRepository
    {
        public ResultRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }
    }
}
