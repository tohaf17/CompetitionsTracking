using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Repositories.Interfaces
{
    public interface IDisciplineRepository : IRepository<Discipline>
    {
        Task<CompetitionsTracking.Application.DTOs.Discipline.DisciplineStatsDto?> GetDisciplineStatsAsync(int disciplineId);
    }
}
