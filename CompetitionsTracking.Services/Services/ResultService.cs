using CompetitionsTracking.Application.DTOs.Result;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Implementations
{
    public class ResultService : IResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IResultRepository _repository;

        public ResultService(IUnitOfWork unitOfWork, IResultRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<ResultResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Adapt<IEnumerable<ResultResponseDto>>();
        }

        public async Task<ResultResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.Adapt<ResultResponseDto>();
        }

        public async Task<ResultResponseDto> CreateAsync(ResultRequestDto request)
        {
            var entity = request.Adapt<Result>();
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Adapt<ResultResponseDto>();
        }

        public async Task UpdateAsync(int id, ResultRequestDto request)
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

        public async Task<IEnumerable<TeamMedalTallyDto>> GetTeamMedalTallyAsync(int competitionId)
        {
            return await _repository.GetTeamMedalTallyAsync(competitionId);
        }
        public async Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync(int competitionId, int disciplineId, int categoryId)
        {
            var results = await _repository.GetLeaderboardAsync(competitionId, disciplineId, categoryId);

            return results.Select(r => new LeaderboardEntryDto
            {
                Place = r.Place,
                ParticipantName = GetParticipantName(r.Entry.Participant),
                Country = GetParticipantCountry(r.Entry.Participant),
                FinalScore = r.FinalScore
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
