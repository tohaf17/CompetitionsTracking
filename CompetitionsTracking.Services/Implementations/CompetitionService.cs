using CompetitionsTracking.Application.DTOs.Competition;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;

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

        public async Task<IEnumerable<CompetitionResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Adapt<IEnumerable<CompetitionResponseDto>>();
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
    }
}
