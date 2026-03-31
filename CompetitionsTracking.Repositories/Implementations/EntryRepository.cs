using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;

namespace CompetitionsTracking.Repositories.Implementations
{
    public class EntryRepository : Repository<Entry>, IEntryRepository
    {
        public EntryRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }
    }
}
