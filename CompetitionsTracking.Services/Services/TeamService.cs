using CompetitionsTracking.Application.DTOs.Team;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Exceptions;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompetitionsTracking.Infrastructure.Data;

namespace CompetitionsTracking.Services.Implementations
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITeamRepository _repository;
        private readonly IPersonRepository _personRepository;
        private readonly CompetitionsTrackingDbContext _context;

        public TeamService(IUnitOfWork unitOfWork, ITeamRepository repository, IPersonRepository personRepository, CompetitionsTrackingDbContext context)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _personRepository = personRepository;
            _context = context;
        }

        public async Task<IEnumerable<TeamResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllWithCoachAsync();
            return entities.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<TeamResponseDto>> GetAllForUserAsync(int userId, UserRole role)
        {
            if (role == UserRole.Admin || role == UserRole.Guest)
            {
                return await GetAllAsync();
            }

            var coachPersonId = await GetCoachPersonIdAsync(userId);
            var entities = await _repository.GetAllForCoachAsync(coachPersonId);
            
            return entities.Select(e => new TeamResponseDto
            {
                Id = e.Id,
                Name = e.Name,
                CoachId = e.CoachId,
                CoachFullName = e.Coach != null ? $"{e.Coach.Name} {e.Coach.Surname}" : "Не призначено",
                Members = e.Members.Select(m => new TeamMemberDto
                {
                    PersonId = m.Id,
                    FullName = $"{m.Name} {m.Surname}",
                    Country = m.Country
                }).ToList()
            });
        }

        private TeamResponseDto MapToResponseDto(Team e)
        {
            return new TeamResponseDto
            {
                Id = e.Id,
                Name = e.Name,
                CoachId = e.CoachId,
                CoachFullName = e.Coach != null ? $"{e.Coach.Name} {e.Coach.Surname}" : "Не призначено"
            };
        }

        private async Task<int> GetCoachPersonIdAsync(int userId)
        {
            var personId = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.PersonId)
                .FirstOrDefaultAsync();

            if (!personId.HasValue)
            {
                throw new BadRequestException("До акаунта тренера не прив'язано профіль тренера.");
            }

            return personId.Value;
        }

        public async Task<TeamResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetTeamWithMembersAsync(id);
            if (entity == null) return null;
            
            return new TeamResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                CoachId = entity.CoachId,
                CoachFullName = entity.Coach != null ? $"{entity.Coach.Name} {entity.Coach.Surname}" : "Не призначено",
                Members = entity.Members?.Select(m => new TeamMemberDto
                {
                    PersonId = m.Id,
                    FullName = $"{m.Name} {m.Surname}",
                    Country = m.Country
                }).ToList() ?? new List<TeamMemberDto>()
            };
        }

        public async Task<TeamResponseDto> CreateAsync(TeamRequestDto request)
        {
            int coachId = request.CoachId ?? 0;

            if (request.CoachId == null && !string.IsNullOrWhiteSpace(request.NewCoachName) && !string.IsNullOrWhiteSpace(request.NewCoachSurname))
            {
                var newCoach = new Person
                {
                    Name = request.NewCoachName,
                    Surname = request.NewCoachSurname,
                    Type = "Person",
                    Gender = Gender.Male, // Defaulting or could be passed
                    Country = "Unknown" // Defaulting
                };
                await _personRepository.AddAsync(newCoach);
                await _unitOfWork.CompleteAsync();
                coachId = newCoach.Id;
            }
            else if (coachId == 0)
            {
                throw new BadRequestException("Не вказано тренера для команди.");
            }

            var entity = request.Adapt<Team>();
            entity.CoachId = coachId;

            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Adapt<TeamResponseDto>();
        }

        public async Task UpdateAsync(int id, TeamRequestDto request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Team), id);

            request.Adapt(entity);
            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Team), id);

            _repository.Remove(entity);
            await _unitOfWork.CompleteAsync();
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
