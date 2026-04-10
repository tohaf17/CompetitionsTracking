using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Repositories.Interfaces
{
    public interface IPersonRepository : IRepository<Person>
    {
        Task<IEnumerable<ParticipantPerformanceDto>> GetParticipantPerformanceHistoryAsync(int participantId);
        Task<IEnumerable<Person>> GetMenteesAsync(int mentorId);
        Task<Person?> GetPersonWithTeamsAsync(int personId);
    }
}
