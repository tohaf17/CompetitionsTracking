using CompetitionsTracking.Application.DTOs.Competition;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Implementations
{
    public class CompetitionService : ICompetitionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICompetitionRepository _repository;

        public CompetitionService(IUnitOfWork unitOfWork, ICompetitionRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        
        public async Task<CompetitionResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.Adapt<CompetitionResponseDto>();
        }

        public async Task<CompetitionResponseDto> CreateAsync(CompetitionRequestDto request)
        {
            var entity = request.Adapt<Competition>();
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Adapt<CompetitionResponseDto>();
        }

        public async Task UpdateAsync(int id, CompetitionRequestDto request)
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

        public async Task<IEnumerable<LeaderboardDto>> GetCompetitionLeaderboardAsync(int competitionId)
        {
            return await _repository.GetCompetitionLeaderboardAsync(competitionId);
        }
        // Змінюємо метод GetAllAsync
        public async Task<IEnumerable<CompetitionResponseDto>> GetAllAsync(CompetitionFilterDto? filter = null)
        {
            var entities = filter == null
                ? await _repository.GetAllAsync()
                : await _repository.GetFilteredAsync(filter);

            return entities.Adapt<IEnumerable<CompetitionResponseDto>>();
        }

        // Нові методи
        public async Task ChangeStatusAsync(int id, ChangeCompetitionStatusDto request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Змагання не знайдено");

            // Бізнес-логіка: статуси змінюються лише в певному порядку, але для простоти просто присвоїмо
            entity.Status = request.NewStatus;

            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task AwardMedalsAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Змагання не знайдено");

            // Бізнес-правило: нагороджуємо тільки якщо змагання завершене
            if (entity.Status != CompetitionStatus.Finished)
                throw new InvalidOperationException("Неможливо розподілити медалі, поки змагання не завершено.");

            await _repository.AwardMedalsAsync(id);
            // Тут не потрібен _unitOfWork.CompleteAsync(), бо ExecuteSqlRawAsync виконується миттєво
        }

        public async Task<CompetitionSummaryDto?> GetSummaryAsync(int id)
        {
            return await _repository.GetSummaryAsync(id);
        }
    }
}
