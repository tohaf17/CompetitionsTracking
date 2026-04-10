using CompetitionsTracking.Application.DTOs.Judge;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Implementations
{
    public class JudgeService : IJudgeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJudgeRepository _repository;

        public JudgeService(IUnitOfWork unitOfWork, IJudgeRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<JudgeResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Adapt<IEnumerable<JudgeResponseDto>>();
        }

        public async Task<JudgeResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.Adapt<JudgeResponseDto>();
        }

        public async Task<JudgeResponseDto> CreateAsync(JudgeRequestDto request)
        {
            var entity = request.Adapt<Judge>();
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Adapt<JudgeResponseDto>();
        }

        public async Task UpdateAsync(int id, JudgeRequestDto request)
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

        public async Task<IEnumerable<JudgeAnalyticsDto>> GetJudgeAnalyticsAsync(int judgeId)
        {
            return await _repository.GetJudgeAnalyticsAsync(judgeId);
        }
        public async Task<IEnumerable<PendingEvaluationDto>> GetPendingEvaluationsAsync(int judgeId, int competitionId)
        {
            var entries = await _repository.GetPendingEvaluationsAsync(judgeId, competitionId);

            return entries.Select(e => new PendingEvaluationDto
            {
                EntryId = e.Id,
                ParticipantName = GetParticipantName(e.Participant),
                DisciplineName = e.Discipline?.Type ?? "Unknown",
                CategoryName = e.Category?.Type ?? "Unknown"
            });
        }

        public async Task<IEnumerable<ConflictOfInterestDto>> GetConflictsOfInterestAsync(int judgeId)
        {
            var conflicts = await _repository.GetConflictsOfInterestAsync(judgeId);

            return conflicts.Select(c => new ConflictOfInterestDto
            {
                ScoreId = c.Id,
                EntryId = c.EntryId,
                ParticipantName = GetParticipantName(c.Entry.Participant),
                // Безпечно витягуємо країну, оскільки конфлікт можливий тільки якщо учасник - Person
                SharedAttribute = c.Entry.Participant is Person p ? $"Спільна країна: {p.Country}" : "Невідомо",
                GivenScore = c.ScoreValue,
                ScoreType = c.Type.ToString() // Мапимо Enum (D, E, A, Penalty) у рядок
            });
        }

        public async Task<IEnumerable<JudgeWorkloadDto>> GetWorkloadSummaryAsync(int judgeId, int competitionId)
        {
            var workload = await _repository.GetWorkloadSummaryAsync(judgeId, competitionId);

            var result = new List<JudgeWorkloadDto>();
            foreach (var item in workload)
            {
                result.Add(new JudgeWorkloadDto
                {
                    DisciplineName = item.DisciplineName,
                    ScoresGiven = item.ScoresGiven
                });
            }
            return result;
        }

        public async Task<IEnumerable<JudgeScoreHistoryDto>> GetJudgeScoresInCompetitionAsync(int judgeId, int competitionId)
        {
            var scores = await _repository.GetJudgeScoresInCompetitionAsync(judgeId, competitionId);

            return scores.Select(s => new JudgeScoreHistoryDto
            {
                ScoreId = s.Id,
                EntryId = s.EntryId,
                ParticipantName = GetParticipantName(s.Entry.Participant),
                ScoreValue = s.ScoreValue,
                ScoreType = s.Type.ToString() // Відображаємо, за що саме була поставлена ця оцінка
            });
        }

        // ПОМІЧНИК ДЛЯ ВИЗНАЧЕННЯ ТИПУ УЧАСНИКА
        private string GetParticipantName(Participant participant)
        {
            return participant switch
            {
                Person p => $"{p.Name} {p.Surname}",
                Team t => $"Команда: {t.Name}",
                _ => "Невідомий учасник"
            };
        }
    }
}
