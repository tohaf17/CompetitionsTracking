using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsTracking.Repositories.Implementations
{
    public class AppealRepository : Repository<Appeal>, IAppealRepository
    {
        public AppealRepository(CompetitionsTrackingDbContext context) : base(context)
        {
        }

        public async Task ApproveAppealWithRecalculationAsync(int appealId, int scoreId, float newScoreValue)
        {
            var parameterAppealId = new SqlParameter("@AppealId", appealId);
            var parameterScoreId = new SqlParameter("@ScoreId", scoreId);
            var parameterNewScore = new SqlParameter("@NewScore", newScoreValue);

            string query = @"
                -- 1. Update Appeal
                UPDATE appeals SET Status = 'Approved', ResolvedAt = GETUTCDATE() WHERE Id = @AppealId;

                -- 2. Update specific score
                UPDATE scores SET ScoreValue = @NewScore WHERE Id = @ScoreId;

                -- 3. Recalculate FinalScore for the entry of this score
                DECLARE @EntryId INT = (SELECT TOP 1 EntryId FROM scores WHERE Id = @ScoreId);

                UPDATE results 
                SET FinalScore = (SELECT SUM(ScoreValue) FROM scores WHERE EntryId = @EntryId)
                WHERE EntryId = @EntryId;

                -- 4. Recalculate Places for all entries in the same Category + Discipline
                DECLARE @CategoryId INT;
                DECLARE @DisciplineId INT;
                SELECT TOP 1 @CategoryId = CategoryId, @DisciplineId = DisciplineId FROM entries WHERE Id = @EntryId;

                WITH RankedResults AS (
                    SELECT r.Id as ResultId, ROW_NUMBER() OVER(ORDER BY r.FinalScore DESC) as NewPlace
                    FROM results r
                    JOIN entries e ON r.EntryId = e.Id
                    WHERE e.CategoryId = @CategoryId AND e.DisciplineId = @DisciplineId
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
            var result = await _context.Set<Result>()
                .Include(r => r.Entry)
                    .ThenInclude(e => e.Competition)
                .FirstOrDefaultAsync(r => r.Id == resultId);

            if (result == null || result.Entry == null || result.Entry.Competition == null) return false;

            return result.Entry.Competition.Status == CompetitionStatus.Ongoing;
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
                .Include(a => a.Result)
                    .ThenInclude(r => r.Entry)
                        .ThenInclude(e => e.Scores)
                            .ThenInclude(s => s.Judge)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
