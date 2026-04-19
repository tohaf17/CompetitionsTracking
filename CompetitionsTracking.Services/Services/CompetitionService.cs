using CompetitionsTracking.Application.DTOs.Common;
using CompetitionsTracking.Application.DTOs.Competition;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Exceptions;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;
using System.Collections.Generic;
using System.Linq;
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
            if (entity == null) throw new NotFoundException(nameof(Competition), id);
            return entity.Adapt<CompetitionResponseDto>();
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
            if (entity == null) throw new NotFoundException(nameof(Competition), id);
            
            request.Adapt(entity);
            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Competition), id);
            
            _repository.Remove(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<LeaderboardDto>> GetCompetitionLeaderboardAsync(int competitionId)
        {
            return await _repository.GetCompetitionLeaderboardAsync(competitionId);
        }

        public async Task<PagedResponse<CompetitionResponseDto>> GetAllAsync(CompetitionFilterDto? filter = null, PaginationParams? pagination = null)
        {
            pagination ??= new PaginationParams();
            IEnumerable<Competition> entities;
            int totalCount;

            if (filter is { Status: not null } || !string.IsNullOrWhiteSpace(filter?.City))
            {
                var filtered = (await _repository.GetFilteredAsync(filter!)).ToList();
                totalCount = filtered.Count;
                entities = filtered
                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize);
            }
            else
            {
                (entities, totalCount) = await _repository.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
            }

            var dtos = entities.Adapt<IEnumerable<CompetitionResponseDto>>();
            return new PagedResponse<CompetitionResponseDto>(dtos, totalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task ChangeStatusAsync(int id, ChangeCompetitionStatusDto request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Competition), id);

            entity.Status = request.NewStatus;

            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task AwardMedalsAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Competition), id);

            if (entity.Status != CompetitionStatus.Finished)
                throw new BadRequestException("Неможливо розподілити медалі, поки змагання не завершено.");

            await _repository.AwardMedalsAsync(id);
        }

        public async Task<CompetitionSummaryDto?> GetSummaryAsync(int id)
        {
            var summary = await _repository.GetSummaryAsync(id);
            if (summary == null) throw new NotFoundException(nameof(Competition), id);
            return summary;
        }
    }
}
