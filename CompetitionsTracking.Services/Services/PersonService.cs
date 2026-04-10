using CompetitionsTracking.Application.DTOs.Person;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Implementations
{
    public class PersonService : IPersonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPersonRepository _repository;

        public PersonService(IUnitOfWork unitOfWork, IPersonRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<PersonResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Adapt<IEnumerable<PersonResponseDto>>();
        }

        public async Task<PersonResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.Adapt<PersonResponseDto>();
        }

        public async Task<PersonResponseDto> CreateAsync(PersonRequestDto request)
        {
            var entity = request.Adapt<Person>();
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Adapt<PersonResponseDto>();
        }

        public async Task UpdateAsync(int id, PersonRequestDto request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                request.Adapt(entity);
                _repository.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                _repository.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<ParticipantPerformanceDto>> GetParticipantPerformanceHistoryAsync(int participantId)
        {
            return await _repository.GetParticipantPerformanceHistoryAsync(participantId);
        }
        public async Task<IEnumerable<MenteeSummaryDto>> GetMenteesAsync(int mentorId)
        {
            var mentees = await _repository.GetMenteesAsync(mentorId);

            return mentees.Select(m => new MenteeSummaryDto
            {
                PersonId = m.Id,
                FullName = $"{m.Name} {m.Surname}",
                Country = m.Country
            });
        }

        public async Task<IEnumerable<TeamAffiliationDto>> GetTeamAffiliationsAsync(int personId)
        {
            var person = await _repository.GetPersonWithTeamsAsync(personId);
            var affiliations = new List<TeamAffiliationDto>();

            if (person == null) return affiliations;

            // Додаємо команди, де людина є тренером
            if (person.TeamsCoached != null && person.TeamsCoached.Any())
            {
                affiliations.AddRange(person.TeamsCoached.Select(t => new TeamAffiliationDto
                {
                    TeamId = t.Id,
                    TeamName = t.Name,
                    Role = "Coach"
                }));
            }

            // Додаємо команди, де людина є учасником
            if (person.TeamsAsMember != null && person.TeamsAsMember.Any())
            {
                affiliations.AddRange(person.TeamsAsMember.Select(t => new TeamAffiliationDto
                {
                    TeamId = t.Id,
                    TeamName = t.Name,
                    Role = "Member"
                }));
            }

            return affiliations;
        }
    }
}
