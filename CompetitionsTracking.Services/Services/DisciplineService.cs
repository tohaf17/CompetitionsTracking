using CompetitionsTracking.Application.DTOs.Discipline;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;

namespace CompetitionsTracking.Services.Implementations
{
    public class DisciplineService : IDisciplineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDisciplineRepository _repository;

        public DisciplineService(IUnitOfWork unitOfWork, IDisciplineRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<DisciplineResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Adapt<IEnumerable<DisciplineResponseDto>>();
        }

        public async Task<DisciplineResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.Adapt<DisciplineResponseDto>();
        }

        public async Task<DisciplineResponseDto> CreateAsync(DisciplineRequestDto request)
        {
            var entity = request.Adapt<Discipline>();
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Adapt<DisciplineResponseDto>();
        }

        public async Task UpdateAsync(int id, DisciplineRequestDto request)
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
