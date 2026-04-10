using CompetitionsTracking.Application.DTOs.Person;
using CompetitionsTracking.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IPersonService
    {
        Task<IEnumerable<PersonResponseDto>> GetAllAsync();
        Task<PersonResponseDto?> GetByIdAsync(int id);
        Task<PersonResponseDto> CreateAsync(PersonRequestDto request);
        Task UpdateAsync(int id, PersonRequestDto request);
        Task DeleteAsync(int id);
        Task<IEnumerable<ParticipantPerformanceDto>> GetParticipantPerformanceHistoryAsync(int participantId);
        Task<IEnumerable<MenteeSummaryDto>> GetMenteesAsync(int mentorId);
        Task<IEnumerable<TeamAffiliationDto>> GetTeamAffiliationsAsync(int personId);
    }
}

