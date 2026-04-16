using CompetitionsTracking.Application.DTOs.Common;
using CompetitionsTracking.Application.DTOs.Entry;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Exceptions;
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

        public async Task<PagedResponse<EntryResponseDto>> GetAllAsync(PaginationParams? pagination = null)
        {
            pagination ??= new PaginationParams();
            var (entities, totalCount) = await _repository.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
            
            var dtos = entities.Adapt<IEnumerable<EntryResponseDto>>();
            return new PagedResponse<EntryResponseDto>(dtos, totalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task<EntryResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Entry), id);
            return entity.Adapt<EntryResponseDto>();
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
            if (entity == null) throw new NotFoundException(nameof(Entry), id);

            request.Adapt(entity);
            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Entry), id);

            _repository.Remove(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<ControversialEntryDto>> GetControversialEntriesAsync(int competitionId)
        {
            return await _repository.GetControversialEntriesAsync(competitionId);
        }

        public async Task<int> BulkUpdateAppStatusAsync(BulkUpdateAppStatusDto request)
        {
            int updatedCount = await _repository.BulkUpdateAppStatusAsync(request.CompetitionId, request.CategoryId, request.NewStatus);
            return updatedCount;
        }

        public async Task ChangeEntryStatusAsync(int id, ChangeEntryStatusDto request)
        {
            var entry = await _repository.GetByIdAsync(id);
            if (entry == null) throw new NotFoundException(nameof(Entry), id);

            entry.EntryStatus = request.NewStatus;
            _repository.Update(entry);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DisqualifyAsync(int entryId)
        {
            var entry = await _repository.GetEntryWithResultAsync(entryId);
            if (entry == null) throw new NotFoundException(nameof(Entry) , entryId);

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
            if (entry == null) throw new NotFoundException(nameof(Entry), entryId);

            var isDuplicate = await _repository.IsDuplicateEntryAsync(entry.CompetitionId, entry.ParticipantId, request.NewDisciplineId);
            if (isDuplicate) throw new BadRequestException("Учасник вже зареєстрований на цю дисципліну.");

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
