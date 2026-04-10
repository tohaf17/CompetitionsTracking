using CompetitionsTracking.Application.DTOs.Score;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Implementations
{
    public class ScoreService : IScoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScoreRepository _repository;

        public ScoreService(IUnitOfWork unitOfWork, IScoreRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<ScoreResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Adapt<IEnumerable<ScoreResponseDto>>();
        }

        public async Task<ScoreResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.Adapt<ScoreResponseDto>();
        }

        public async Task<ScoreResponseDto> CreateAsync(ScoreRequestDto request)
        {
            var entity = request.Adapt<Score>();
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Adapt<ScoreResponseDto>();
        }

        public async Task UpdateAsync(int id, ScoreRequestDto request)
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

        public async Task<IEnumerable<ScoreAnomalyDto>> GetScoreAnomaliesAsync(int competitionId)
        {
            return await _repository.GetScoreAnomaliesAsync(competitionId);
        }
        public async Task<IEnumerable<EntryScoreDetailDto>> GetScoresByEntryAsync(int entryId)
        {
            var scores = await _repository.GetScoresByEntryAsync(entryId);

            return scores.Select(s => new EntryScoreDetailDto
            {
                ScoreId = s.Id,
                JudgeName = $"{s.Judge.Person.Name} {s.Judge.Person.Surname}",
                ScoreType = s.Type.ToString(),
                ScoreValue = s.ScoreValue
            });
        }

        public async Task<EntryScoreBreakdownDto?> GetEntryScoreBreakdownAsync(int entryId)
        {
            var scores = await _repository.GetScoresByEntryAsync(entryId);
            if (!scores.Any()) return null;

            // Логіка підрахунку: D-оцінки сумуються, E та A - усереднюються, Штрафи - віднімаються
            // (За потреби адаптуй під реальні правила твого виду спорту)
            var difficulty = scores.Where(s => s.Type == Domain.Entities.ScoreType.D).Sum(s => s.ScoreValue);

            var executionScores = scores.Where(s => s.Type == Domain.Entities.ScoreType.E || s.Type == Domain.Entities.ScoreType.A).ToList();
            var avgExecution = executionScores.Any() ? executionScores.Average(s => s.ScoreValue) : 0f;

            var penalties = scores.Where(s => s.Type == Domain.Entities.ScoreType.Penalty).Sum(s => s.ScoreValue);

            var total = difficulty + avgExecution - penalties;

            return new EntryScoreBreakdownDto
            {
                EntryId = entryId,
                TotalDifficulty = difficulty,
                AverageExecution = avgExecution,
                TotalPenalties = penalties,
                CalculatedTotalScore = total
            };
        }
    }
}
