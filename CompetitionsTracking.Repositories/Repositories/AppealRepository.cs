using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsTracking.Repositories.Repositories
{
    public class AppealRepository : Repository<Appeal>, IAppealRepository
    {
        public AppealRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Appeal>> GetAllAsync()
        {
            return await _context.Appeals
                .Include(a => a.Result)
                    .ThenInclude(r => r.Entry)
                        .ThenInclude(e => e.Participant)
                .Include(a => a.Result)
                    .ThenInclude(r => r.Entry)
                        .ThenInclude(e => e.Competition)
                .ToListAsync();
        }

        public async Task ApproveAppealWithRecalculationAsync(int appealId, int scoreId, float newScoreValue)
        {
            var parameterAppealId = new SqlParameter("@AppealId", appealId);
            var parameterScoreId = new SqlParameter("@ScoreId", scoreId);
            var parameterNewScore = new SqlParameter("@NewScore", newScoreValue);

            string query = @"
                UPDATE appeals SET Status = 'Approved', ResolvedAt = GETUTCDATE() WHERE Id = @AppealId;

                UPDATE scores SET ScoreValue = @NewScore WHERE Id = @ScoreId;

                DECLARE @EntryId INT = (SELECT TOP 1 EntryId FROM scores WHERE Id = @ScoreId);

                UPDATE results 
                SET FinalScore = (SELECT SUM(ScoreValue) FROM scores WHERE EntryId = @EntryId)
                WHERE EntryId = @EntryId;

                DECLARE @CategoryId INT;
                DECLARE @DisciplineType NVARCHAR(MAX);
                SELECT TOP 1 @CategoryId = e.CategoryId, @DisciplineType = d.Type
                FROM entries e
                JOIN disciplines d ON e.DisciplineId = d.Id
                WHERE e.Id = @EntryId;

                WITH RankedResults AS (
                    SELECT r.Id as ResultId, DENSE_RANK() OVER(ORDER BY r.FinalScore DESC) as NewPlace
                    FROM results r
                    JOIN entries e ON r.EntryId = e.Id
                    JOIN disciplines d ON e.DisciplineId = d.Id
                    WHERE e.CategoryId = @CategoryId AND d.Type = @DisciplineType
                )
                UPDATE r
                SET r.Place = rr.NewPlace
                FROM results r
                JOIN RankedResults rr ON r.Id = rr.ResultId;
            ";

            await _context.Database.ExecuteSqlRawAsync(query, parameterAppealId, parameterScoreId, parameterNewScore);
        }

        public async Task<bool> HasAppealForResultAsync(int resultId)
        {
            return await _context.Set<Appeal>().AnyAsync(a => a.ResultId == resultId);
        }

        public async Task<bool> IsCompetitionOngoingForResultAsync(int resultId)
        {
            var status = await _context.Set<Result>()
                .Where(r => r.Id == resultId)
                .Select(r => (CompetitionStatus?)r.Entry.Competition.Status)
                .FirstOrDefaultAsync();

            return status == CompetitionStatus.Ongoing;
        }

        public async Task<IEnumerable<Appeal>> GetPendingAppealsAsync(int? competitionId)
        {
            var query = _context.Set<Appeal>()
                .Include(a => a.Result)
                    .ThenInclude(r => r.Entry)
                        .ThenInclude(e => e.Participant)
                .Include(a => a.Result)
                    .ThenInclude(r => r.Entry)
                        .ThenInclude(e => e.Competition)
                .Where(a => a.Status == AppealStatus.Pending);

            if (competitionId.HasValue)
            {
                query = query.Where(a => a.Result.Entry.CompetitionId == competitionId.Value);
            }

            return await query.OrderBy(a => a.CreatedAt).ToListAsync();
        }

        public async Task<Appeal?> GetAppealDossierAsync(int id)
        {
            return await _context.Set<Appeal>()
                .AsNoTracking()
                .AsSplitQuery()
                .Include(a => a.Result)
                    .ThenInclude(r => r.Entry)
                        .ThenInclude(e => e.Scores)
                            .ThenInclude(s => s.Judge)
                                .ThenInclude(j => j.Person)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
