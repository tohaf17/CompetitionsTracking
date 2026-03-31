using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;

namespace CompetitionsTracking.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CompetitionsTrackingDbContext _context;

        public UnitOfWork(CompetitionsTrackingDbContext context)
        {
            _context = context;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
