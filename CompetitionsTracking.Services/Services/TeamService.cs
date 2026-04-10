using CompetitionsTracking.Application.DTOs.Team;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Implementations
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITeamRepository _repository;
        private readonly IPersonRepository _personRepository;

        public TeamService(IUnitOfWork unitOfWork, ITeamRepository repository, IPersonRepository personRepository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _personRepository = personRepository;
        }

        public async Task<IEnumerable<TeamResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Adapt<IEnumerable<TeamResponseDto>>();
        }

        public async Task<TeamResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.Adapt<TeamResponseDto>();
        }

        public async Task<TeamResponseDto> CreateAsync(TeamRequestDto request)
        {
            var entity = request.Adapt<Team>();
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Adapt<TeamResponseDto>();
        }

        public async Task UpdateAsync(int id, TeamRequestDto request)
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

        public async Task<IEnumerable<TeamDominanceMetricDto>> GetTeamDominanceMetricsAsync(int teamId)
        {
            return await _repository.GetTeamDominanceMetricsAsync(teamId);
        }
        public async Task<TeamRosterDto?> GetTeamRosterAsync(int teamId)
        {
            var team = await _repository.GetTeamWithMembersAsync(teamId);
            if (team == null) return null;

            return new TeamRosterDto
            {
                TeamId = team.Id,
                TeamName = team.Name,
                CoachFullName = team.Coach != null ? $"{team.Coach.Name} {team.Coach.Surname}" : "Не призначено",
                Members = team.Members.Select(m => new TeamMemberDto
                {
                    PersonId = m.Id,
                    FullName = $"{m.Name} {m.Surname}",
                    Country = m.Country
                }).ToList()
            };
        }

        public async Task AddMemberToTeamAsync(int teamId, int personId)
        {
            var team = await _repository.GetTeamWithMembersAsync(teamId);
            // Тут ідеально було б ще інжектити IPersonRepository, щоб перевірити, чи існує Person, 
            // але для спрощення припустимо, що ми знаходимо людину через загальний контекст
            var person = await _personRepository.GetByIdAsync(personId);

            if (team != null && person != null && !team.Members.Any(m => m.Id == personId))
            {
                team.Members.Add(person);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task RemoveMemberFromTeamAsync(int teamId, int personId)
        {
            var team = await _repository.GetTeamWithMembersAsync(teamId);

            if (team != null)
            {
                var personToRemove = team.Members.FirstOrDefault(m => m.Id == personId);
                if (personToRemove != null)
                {
                    team.Members.Remove(personToRemove);
                    await _unitOfWork.CompleteAsync();
                }
            }
        }
    }
}
