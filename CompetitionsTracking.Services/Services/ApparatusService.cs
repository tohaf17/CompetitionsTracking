using CompetitionsTracking.Application.DTOs.Apparatus;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Exceptions;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;

namespace CompetitionsTracking.Services.Implementations
{
    public class ApparatusService : IApparatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApparatusRepository _repository;

        public ApparatusService(IUnitOfWork unitOfWork, IApparatusRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<ApparatusResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Adapt<IEnumerable<ApparatusResponseDto>>();
        }

        public async Task<ApparatusResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.Adapt<ApparatusResponseDto>();
        }

        public async Task<ApparatusResponseDto> CreateAsync(ApparatusRequestDto request)
        {
            var entity = request.Adapt<Apparatus>();
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Adapt<ApparatusResponseDto>();
        }

        public async Task UpdateAsync(int id, ApparatusRequestDto request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Apparatus), id);

            request.Adapt(entity);
            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Apparatus), id);

            _repository.Remove(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
