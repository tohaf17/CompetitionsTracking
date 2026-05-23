using CompetitionsTracking.Application.DTOs.Appeal;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Exceptions;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsTracking.Services.Implementations
{
    public class AppealService : IAppealService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppealRepository _repository;
        private readonly CompetitionsTrackingDbContext _context;

        public AppealService(IUnitOfWork unitOfWork, IAppealRepository repository, CompetitionsTrackingDbContext context)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<AppealResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(a => new AppealResponseDto
            {
                Id = a.Id,
                ResultId = a.ResultId,
                Reason = a.Reason,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                ResolvedAt = a.ResolvedAt,
                ParticipantName = a.Result?.Entry?.Participant is Person p 
                    ? $"{p.Name} {p.Surname}" 
                    : (a.Result?.Entry?.Participant is Team t ? t.Name : "Unknown"),
                CompetitionId = a.Result?.Entry?.CompetitionId ?? 0,
                CompetitionName = a.Result?.Entry?.Competition?.Title ?? "Unknown"
            });
        }

        public async Task<IEnumerable<AppealResponseDto>> GetAllForUserAsync(int userId, UserRole role)
        {
            if (role == UserRole.Admin)
            {
                return await GetAllAsync();
            }

            if (role == UserRole.Guest)
            {
                return Enumerable.Empty<AppealResponseDto>();
            }

            var coachPersonId = await GetCoachPersonIdAsync(userId);
            var appeals = await AppealsWithDetails()
                .Where(a =>
                    _context.Persons.Any(p => p.Id == a.Result.Entry.ParticipantId
                        && (p.MentorId == coachPersonId || p.TeamsAsMember.Any(t => t.CoachId == coachPersonId)))
                    || _context.Teams.Any(t => t.Id == a.Result.Entry.ParticipantId && t.CoachId == coachPersonId))
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return appeals.Select(MapToResponseDto);
        }

        public async Task<AppealResponseDto?> GetByIdAsync(int id)
        {
            var a = await _repository.GetByIdAsync(id);
            if (a == null) throw new NotFoundException(nameof(Appeal), id);
            return new AppealResponseDto
            {
                Id = a.Id,
                ResultId = a.ResultId,
                Reason = a.Reason,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                ResolvedAt = a.ResolvedAt,
                ParticipantName = a.Result?.Entry?.Participant is Person p 
                    ? $"{p.Name} {p.Surname}" 
                    : (a.Result?.Entry?.Participant is Team t ? t.Name : "Unknown"),
                CompetitionId = a.Result?.Entry?.CompetitionId ?? 0,
                CompetitionName = a.Result?.Entry?.Competition?.Title ?? "Unknown"
            };
        }

        public async Task<AppealResponseDto> CreateAsync(AppealRequestDto request)
        {
            bool hasDuplicate = await _repository.HasAppealForResultAsync(request.ResultId);
            if (hasDuplicate)
            {
                throw new ConflictException("An appeal already exists for this result.");
            }

            var entity = request.Adapt<Appeal>();
            entity.Status = AppealStatus.Pending; 
            entity.CreatedAt = DateTime.UtcNow;

            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Adapt<AppealResponseDto>();
            
        }

        public async Task<AppealResponseDto> CreateAsync(AppealRequestDto request, int userId, UserRole role)
        {
            if (role != UserRole.Admin)
            {
                if (role == UserRole.Guest)
                {
                    throw new BadRequestException("Гості не можуть подавати апеляції.");
                }
                var result = await _context.Results
                    .Where(r => r.Id == request.ResultId)
                    .Select(r => new
                    {
                        CompetitionLevel = r.Entry.Competition.Level
                    })
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    throw new NotFoundException(nameof(Result), request.ResultId);
                }

                if (result.CompetitionLevel == CompetitionLevel.International)
                {
                    throw new BadRequestException("Тренер не може подавати апеляції на міжнародні змагання. Для міжнародних змагань апеляції подає адміністратор.");
                }

                var coachPersonId = await GetCoachPersonIdAsync(userId);
                var ownsResult = await _context.Results.AnyAsync(r => r.Id == request.ResultId
                    && (_context.Persons.Any(p => p.Id == r.Entry.ParticipantId
                            && (p.MentorId == coachPersonId || p.TeamsAsMember.Any(t => t.CoachId == coachPersonId)))
                        || _context.Teams.Any(t => t.Id == r.Entry.ParticipantId && t.CoachId == coachPersonId)));

                if (!ownsResult)
                {
                    throw new BadRequestException("Тренер може подавати апеляцію лише на виступ свого підопічного або своєї команди.");
                }
            }

            return await CreateAsync(request);
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
                CompetitionName = a.Result.Entry.Competition?.Title ?? "Unknown",
                ParticipantName = a.Result.Entry.Participant is Person p 
                    ? $"{p.Name} {p.Surname}" 
                    : (a.Result.Entry.Participant is Team t ? t.Name : "Unknown Participant")
            });
        }

        public async Task<IEnumerable<PendingAppealDto>> GetPendingAppealsForUserAsync(int? competitionId, int userId, UserRole role)
        {
            if (role == UserRole.Admin)
            {
                return await GetPendingAppealsAsync(competitionId);
            }

            if (role == UserRole.Guest)
            {
                return Enumerable.Empty<PendingAppealDto>();
            }

            var coachPersonId = await GetCoachPersonIdAsync(userId);
            var query = AppealsWithDetails()
                .Where(a => a.Status == AppealStatus.Pending)
                .Where(a =>
                    _context.Persons.Any(p => p.Id == a.Result.Entry.ParticipantId
                        && (p.MentorId == coachPersonId || p.TeamsAsMember.Any(t => t.CoachId == coachPersonId)))
                    || _context.Teams.Any(t => t.Id == a.Result.Entry.ParticipantId && t.CoachId == coachPersonId));

            if (competitionId.HasValue)
            {
                query = query.Where(a => a.Result.Entry.CompetitionId == competitionId.Value);
            }

            var appeals = await query.OrderBy(a => a.CreatedAt).ToListAsync();
            return appeals.Select(MapToPendingDto);
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

        public async Task<AppealDossierDto?> GetAppealDossierAsync(int id, int userId, UserRole role)
        {
            if (role != UserRole.Admin)
            {
                if (role == UserRole.Guest)
                {
                    throw new BadRequestException("Гості не мають доступу до деталей апеляцій.");
                }

                var coachPersonId = await GetCoachPersonIdAsync(userId);
                var ownsAppeal = await _context.Appeals.AnyAsync(a => a.Id == id
                    && (_context.Persons.Any(p => p.Id == a.Result.Entry.ParticipantId
                            && (p.MentorId == coachPersonId || p.TeamsAsMember.Any(t => t.CoachId == coachPersonId)))
                        || _context.Teams.Any(t => t.Id == a.Result.Entry.ParticipantId && t.CoachId == coachPersonId)));

                if (!ownsAppeal)
                {
                    throw new BadRequestException("Немає доступу до цієї апеляції.");
                }
            }

            return await GetAppealDossierAsync(id);
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

        private IQueryable<Appeal> AppealsWithDetails()
        {
            return _context.Appeals
                .Include(a => a.Result)
                    .ThenInclude(r => r.Entry)
                        .ThenInclude(e => e.Participant)
                .Include(a => a.Result)
                    .ThenInclude(r => r.Entry)
                        .ThenInclude(e => e.Competition)
                .AsNoTracking();
        }

        private AppealResponseDto MapToResponseDto(Appeal a)
        {
            return new AppealResponseDto
            {
                Id = a.Id,
                ResultId = a.ResultId,
                Reason = a.Reason,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                ResolvedAt = a.ResolvedAt,
                ParticipantName = GetParticipantName(a.Result?.Entry?.Participant),
                CompetitionId = a.Result?.Entry?.CompetitionId ?? 0,
                CompetitionName = a.Result?.Entry?.Competition?.Title ?? "Unknown"
            };
        }

        private PendingAppealDto MapToPendingDto(Appeal a)
        {
            return new PendingAppealDto
            {
                Id = a.Id,
                ResultId = a.ResultId,
                Reason = a.Reason,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                CompetitionId = a.Result.Entry.CompetitionId,
                CompetitionName = a.Result.Entry.Competition?.Title ?? "Unknown",
                ParticipantName = GetParticipantName(a.Result.Entry.Participant)
            };
        }

        private static string GetParticipantName(Participant? participant)
        {
            return participant switch
            {
                Person p => $"{p.Name} {p.Surname}",
                Team t => t.Name,
                _ => "Unknown"
            };
        }

        private async Task<int> GetCoachPersonIdAsync(int userId)
        {
            var personId = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.PersonId)
                .FirstOrDefaultAsync();

            if (!personId.HasValue)
            {
                throw new BadRequestException("До акаунта тренера не прив'язано профіль тренера.");
            }

            return personId.Value;
        }
    }
}
