using CompetitionsTracking.Application.DTOs.Judge;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;

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
    }
}
