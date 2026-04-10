using CompetitionsTracking.Application.DTOs.Appeal;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Exceptions;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;

namespace CompetitionsTracking.Services.Implementations
{
    public class AppealService : IAppealService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppealRepository _repository;

        public AppealService(IUnitOfWork unitOfWork, IAppealRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<AppealResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Adapt<IEnumerable<AppealResponseDto>>();
        }

        public async Task<AppealResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Appeal), id);
            return entity.Adapt<AppealResponseDto>();
        }

        public async Task<AppealResponseDto> CreateAsync(AppealRequestDto request)
        {
            bool hasDuplicate = await _repository.HasAppealForResultAsync(request.ResultId);
            if (hasDuplicate)
            {
                throw new ConflictException("An appeal already exists for this result.");
            }

            bool isOngoing = await _repository.IsCompetitionOngoingForResultAsync(request.ResultId);
            if (!isOngoing)
            {
                throw new BadRequestException("Appeals can only be submitted while the competition is ongoing.");
            }

            var entity = request.Adapt<Appeal>();
            entity.Status = AppealStatus.Pending; 
            entity.CreatedAt = DateTime.UtcNow;

            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Adapt<AppealResponseDto>();
            
        }

        public async Task<IEnumerable<PendingAppealDto>> GetPendingAppealsAsync(int? competitionId)
        {
            var appeals = await _repository.GetPendingAppealsAsync(competitionId);
            return appeals.Select(a => new PendingAppealDto
            {
                Id = a.Id,
                ResultId = a.ResultId,
                Reason = a.Reason,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                CompetitionId = a.Result.Entry.CompetitionId,
                ParticipantName = a.Result.Entry.Participant is Person p 
                    ? $"{p.Name} {p.Surname}" 
                    : (a.Result.Entry.Participant is Team t ? t.Name : "Unknown Participant")
            });
        }

        public async Task<AppealDossierDto?> GetAppealDossierAsync(int id)
        {
            var appeal = await _repository.GetAppealDossierAsync(id);
            if (appeal == null) throw new NotFoundException(nameof(Appeal), id);

            return new AppealDossierDto
            {
                AppealId = appeal.Id,
                Reason = appeal.Reason,
                Status = appeal.Status,
                FinalScore = appeal.Result.FinalScore,
                Scores = appeal.Result.Entry.Scores.Select(s => new DossierScoreDto
                {
                    ScoreId = s.Id,
                    Value = s.ScoreValue,
                    ScoreType = s.Type.ToString(),
                    JudgeName = s.Judge?.Person != null ? $"{s.Judge.Person.Name} {s.Judge.Person.Surname}" : "Unknown Judge"
                }).ToList()
            };
        }

        public async Task UpdateAsync(int id, AppealRequestDto request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Appeal), id);
            
            request.Adapt(entity);
            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Appeal), id);
            
            _repository.Remove(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task ApproveAppealAsync(int id, ApproveAppealRequestDto request)
        {
            await _repository.ApproveAppealWithRecalculationAsync(id, request.ScoreIdToEdit, request.NewScoreValue);
        }
    }
}
