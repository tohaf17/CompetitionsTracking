using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;

namespace CompetitionsTracking.Repositories.Implementations
{
    public class JudgeRepository : Repository<Judge>, IJudgeRepository
    {
        public JudgeRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }
    }
}
