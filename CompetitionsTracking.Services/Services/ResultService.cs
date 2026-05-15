using CompetitionsTracking.Application.DTOs.Result;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Exceptions;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using CompetitionsTracking.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Implementations
{
    public class ResultService : IResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IResultRepository _repository;
        private readonly CompetitionsTrackingDbContext _context;

        public ResultService(IUnitOfWork unitOfWork, IResultRepository repository, CompetitionsTrackingDbContext context)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<ResultResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<ResultResponseDto>> GetAppealableForUserAsync(int userId, UserRole role)
        {
            var query = _context.Results
                .Include(r => r.Entry).ThenInclude(e => e.Participant)
                .Include(r => r.Entry).ThenInclude(e => e.Competition)
                .Where(r => r.Entry.Competition.Status == CompetitionStatus.Ongoing)
                .Where(r => !r.Appeals.Any(a => a.Status == AppealStatus.Pending));

            if (role == UserRole.Admin)
            {
                // Admin sees all
            }
            else if (role == UserRole.Guest)
            {
                return Enumerable.Empty<ResultResponseDto>();
            }
            else // Trainee
            {
                var coachPersonId = await GetCoachPersonIdAsync(userId);
                query = query
                    .Where(r => r.Entry.Competition.Level != CompetitionLevel.International)
                    .Where(r => _context.Persons.Any(p => p.Id == r.Entry.ParticipantId
                        && (p.MentorId == coachPersonId || p.TeamsAsMember.Any(t => t.CoachId == coachPersonId)))
                    || _context.Teams.Any(t => t.Id == r.Entry.ParticipantId && t.CoachId == coachPersonId));
            }

            var results = await query
                .AsNoTracking()
                .OrderBy(r => r.Entry.Competition.Title)
                .ThenBy(r => r.Place)
                .ToListAsync();

            return results.Select(MapToResponseDto);
        }

        public async Task<ResultResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new ResultResponseDto
            {
                Id = entity.Id,
                EntryId = entity.EntryId,
                Place = entity.Place,
                FinalScore = entity.FinalScore,
                AwardedMedal = entity.AwardedMedal ?? string.Empty,
                ParticipantName = entity.Entry != null ? GetParticipantName(entity.Entry.Participant) : "Unknown",
                CompetitionId = entity.Entry?.CompetitionId ?? 0,
                CompetitionName = entity.Entry?.Competition?.Title ?? "Unknown",
                CompetitionStatus = entity.Entry?.Competition?.Status ?? CompetitionStatus.Planned,
                CompetitionLevel = entity.Entry?.Competition?.Level ?? CompetitionLevel.National
            };
        }

        private ResultResponseDto MapToResponseDto(Result e)
        {
            return new ResultResponseDto
            {
                Id = e.Id,
                EntryId = e.EntryId,
                Place = e.Place,
                FinalScore = e.FinalScore,
                AwardedMedal = e.AwardedMedal ?? string.Empty,
                ParticipantName = e.Entry != null ? GetParticipantName(e.Entry.Participant) : "Unknown",
                CompetitionId = e.Entry?.CompetitionId ?? 0,
                CompetitionName = e.Entry?.Competition?.Title ?? "Unknown",
                CompetitionStatus = e.Entry?.Competition?.Status ?? CompetitionStatus.Planned,
                CompetitionLevel = e.Entry?.Competition?.Level ?? CompetitionLevel.National
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

        public async Task<ResultResponseDto> CreateAsync(ResultRequestDto request)
        {
            var entity = request.Adapt<Result>();
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            var created = await _repository.GetByIdAsync(entity.Id);
            return created != null ? new ResultResponseDto
            {
                Id = created.Id,
                EntryId = created.EntryId,
                Place = created.Place,
                FinalScore = created.FinalScore,
                AwardedMedal = created.AwardedMedal ?? string.Empty,
                ParticipantName = created.Entry != null ? GetParticipantName(created.Entry.Participant) : "Unknown",
                CompetitionId = created.Entry?.CompetitionId ?? 0,
                CompetitionName = created.Entry?.Competition?.Title ?? "Unknown",
                CompetitionStatus = created.Entry?.Competition?.Status ?? CompetitionStatus.Planned,
                CompetitionLevel = created.Entry?.Competition?.Level ?? CompetitionLevel.National
            } : entity.Adapt<ResultResponseDto>();
        }

        public async Task UpdateAsync(int id, ResultRequestDto request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Result), id);

            request.Adapt(entity);
            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Result), id);

            _repository.Remove(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<TeamMedalTallyDto>> GetTeamMedalTallyAsync(int competitionId)
        {
            return await _repository.GetTeamMedalTallyAsync(competitionId);
        }
        public async Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync(int competitionId, int? disciplineId, int? categoryId)
        {
            var results = await _repository.GetLeaderboardAsync(competitionId, disciplineId, categoryId);

            return results.Select(r => new LeaderboardEntryDto
            {
                EntryId = r.EntryId,
                ParticipantId = r.Entry.ParticipantId,
                ParticipantType = r.Entry.Participant?.Type ?? string.Empty,
                Place = r.Place,
                ParticipantName = GetParticipantName(r.Entry.Participant),
                Country = GetParticipantCountry(r.Entry.Participant),
                DisciplineName = r.Entry.Discipline?.Type ?? "Unknown",
                CategoryName = r.Entry.Category?.Type ?? "Unknown",
                FinalScore = r.FinalScore,
                AwardedMedal = r.AwardedMedal ?? string.Empty
            });
        }

        public async Task<IEnumerable<CountryMedalTallyDto>> GetCountryMedalTallyAsync(int competitionId)
        {
            var medalists = await _repository.GetMedalistsByCompetitionAsync(competitionId);

            var tally = medalists
                .Select(m => new
                {
                    Country = GetParticipantCountry(m.Entry.Participant),
                    Place = m.Place
                })
                .Where(x => !string.IsNullOrEmpty(x.Country)) 
                .GroupBy(x => x.Country)
                .Select(g => new CountryMedalTallyDto
                {
                    Country = g.Key,
                    Gold = g.Count(x => x.Place == 1),
                    Silver = g.Count(x => x.Place == 2),
                    Bronze = g.Count(x => x.Place == 3)
                })
                .OrderByDescending(t => t.Gold)
                .ThenByDescending(t => t.Silver)
                .ThenByDescending(t => t.Bronze)
                .ToList();

            return tally;
        }

        public async Task<IEnumerable<DisciplineRecordDto>> GetTopRecordsByDisciplineAsync(int disciplineId, int topN = 10)
        {
            var records = await _repository.GetTopRecordsByDisciplineAsync(disciplineId, topN);

            return records.Select(r => new DisciplineRecordDto
            {
                ParticipantName = GetParticipantName(r.Entry.Participant),
                CompetitionName = r.Entry.Competition?.Title?? "Unknown",
                FinalScore = r.FinalScore,
                CompetitionDate = r.Entry.Competition?.StartDate ?? System.DateTime.MinValue
            });
        }

        private string GetParticipantName(Participant participant)
        {
            return participant switch
            {
                Person p => $"{p.Name} {p.Surname}",
                Team t => t.Name,
                _ => "Unknown"
            };
        }

        private string GetParticipantCountry(Participant participant)
        {
            return participant switch
            {
                Person p => p.Country,
                _ => string.Empty
            };
        }
    }
}
