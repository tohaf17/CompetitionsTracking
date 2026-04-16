using CompetitionsTracking.Application.DTOs.Score;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;

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

            var difficulty = scores
                .Where(s => s.Type == ScoreType.D || 
                            s.Type == ScoreType.DA || 
                            s.Type == ScoreType.DB)
                .Sum(s => s.ScoreValue);
            
            var executionScores = scores.Where(s => s.Type == ScoreType.E).ToList();
            var avgExecution = executionScores.Any() ? executionScores.Average(s => s.ScoreValue) : 0f;

            var artistryScores = scores.Where(s => s.Type ==ScoreType.A).ToList();
            var avgArtistry = artistryScores.Any() ? artistryScores.Average(s => s.ScoreValue) : 0f;

            var penalties = scores.Where(s => s.Type == ScoreType.Penalty).Sum(s => s.ScoreValue);

            var total = difficulty + avgExecution + avgArtistry - penalties;

            return new EntryScoreBreakdownDto
            {
                EntryId = entryId,
                TotalDifficulty = difficulty,
                AverageExecution = avgExecution,
                AverageArtistry = avgArtistry,
                TotalPenalties = penalties,
                CalculatedTotalScore = total
            };
        }

    }
}
