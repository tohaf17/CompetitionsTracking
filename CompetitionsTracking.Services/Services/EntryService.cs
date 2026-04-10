using CompetitionsTracking.Application.DTOs.Entry;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Implementations
{
    public class EntryService : IEntryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntryRepository _repository;

        public EntryService(IUnitOfWork unitOfWork, IEntryRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<EntryResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Adapt<IEnumerable<EntryResponseDto>>();
        }

        public async Task<EntryResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.Adapt<EntryResponseDto>();
        }

        public async Task<EntryResponseDto> CreateAsync(EntryRequestDto request)
        {
            var entity = request.Adapt<Entry>();
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Adapt<EntryResponseDto>();
        }

        public async Task UpdateAsync(int id, EntryRequestDto request)
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

        public async Task<IEnumerable<ControversialEntryDto>> GetControversialEntriesAsync(int competitionId)
        {
            return await _repository.GetControversialEntriesAsync(competitionId);
        }
        public async Task<int> BulkUpdateAppStatusAsync(BulkUpdateAppStatusDto request)
        {
            return await _repository.BulkUpdateAppStatusAsync(request.CompetitionId, request.CategoryId, request.NewStatus);
        }

        public async Task ChangeEntryStatusAsync(int id, ChangeEntryStatusDto request)
        {
            var entry = await _repository.GetByIdAsync(id);
            if (entry == null) throw new KeyNotFoundException("Заявку не знайдено");

            entry.EntryStatus = request.NewStatus;
            _repository.Update(entry);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DisqualifyAsync(int entryId)
        {
            var entry = await _repository.GetEntryWithResultAsync(entryId);
            if (entry == null) throw new KeyNotFoundException("Заявку не знайдено");

            // Використовуємо існуючий статус DNS або можеш додати DSQ у свій enum EntryStatus
            entry.EntryStatus = EntryStatus.DNS;

            if (entry.Result != null)
            {
                entry.Result.FinalScore = 0;
            }

            _repository.Update(entry);
            await _unitOfWork.CompleteAsync();
        }

        public async Task TransferEntryAsync(int entryId, TransferEntryDto request)
        {
            var entry = await _repository.GetByIdAsync(entryId);
            if (entry == null) throw new KeyNotFoundException("Заявку не знайдено");

            var isDuplicate = await _repository.IsDuplicateEntryAsync(entry.CompetitionId, entry.ParticipantId, request.NewDisciplineId);
            if (isDuplicate) throw new System.InvalidOperationException("Учасник вже зареєстрований на цю дисципліну.");

            entry.CategoryId = request.NewCategoryId;
            entry.DisciplineId = request.NewDisciplineId;

            _repository.Update(entry);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<EntryResponseDto>> GetStartListAsync(int competitionId)
        {
            var entries = await _repository.GetStartListAsync(competitionId);
            return entries.Adapt<IEnumerable<EntryResponseDto>>();
        }

        public async Task<IEnumerable<EntryResponseDto>> GetMissingScoresAsync(int competitionId, int expectedCount)
        {
            var entries = await _repository.GetEntriesWithMissingScoresAsync(competitionId, expectedCount);
            return entries.Adapt<IEnumerable<EntryResponseDto>>();
        }

        public async Task<EntryAnalyticsDto> GetAnalyticsAsync(int competitionId)
        {
            return await _repository.GetEntryAnalyticsAsync(competitionId);
        }
    }
}
