using CompetitionsTracking.Application.DTOs.Common;
using CompetitionsTracking.Application.DTOs.Entry;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Exceptions;
using CompetitionsTracking.Domain.Models;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsTracking.Services.Implementations
{
    public class EntryService : IEntryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntryRepository _repository;
        private readonly CompetitionsTrackingDbContext _context;

        public EntryService(IUnitOfWork unitOfWork, IEntryRepository repository, CompetitionsTrackingDbContext context)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _context = context;
        }

        public async Task<PagedResponse<EntryResponseDto>> GetAllAsync(PaginationParams? pagination = null)
        {
            pagination ??= new PaginationParams();
            var (entities, totalCount) = await _repository.GetPagedWithDetailsAsync(pagination.PageNumber, pagination.PageSize);
            
            var dtos = entities.Select(e => MapToResponseDto(e));
            return new PagedResponse<EntryResponseDto>(dtos, totalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task<PagedResponse<EntryResponseDto>> GetAllForUserAsync(PaginationParams? pagination, int userId, bool isAdmin)
        {
            if (isAdmin)
            {
                return await GetAllAsync(pagination);
            }

            pagination ??= new PaginationParams();
            var coachPersonId = await GetCoachPersonIdAsync(userId);
            var query = RestrictToCoach(EntriesWithDetails(), coachPersonId.Value);

            var totalCount = await query.CountAsync();
            var entities = await query
                .OrderByDescending(e => e.SubmittedAt)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResponse<EntryResponseDto>(
                entities.Select(MapToResponseDto),
                totalCount,
                pagination.PageNumber,
                pagination.PageSize);
        }

        public async Task<EntryResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new NotFoundException(nameof(Entry), id);
            return MapToResponseDto(entity);
        }

        private EntryResponseDto MapToResponseDto(Entry entity)
        {
            return new EntryResponseDto
            {
                Id = entity.Id,
                CompetitionId = entity.CompetitionId,
                CompetitionName = entity.Competition?.Title ?? "Unknown",
                ParticipantId = entity.ParticipantId,
                ParticipantType = entity.Participant?.Type ?? string.Empty,
                ParticipantName = GetParticipantName(entity.Participant),
                TeamName = GetTeamName(entity.Participant),
                DisciplineId = entity.DisciplineId,
                DisciplineName = entity.Discipline?.Type ?? "Unknown",
                CategoryId = entity.CategoryId,
                CategoryName = entity.Category?.Type ?? "Unknown",
                ApplicationStatus = entity.ApplicationStatus,
                EntryStatus = entity.EntryStatus,
                SubmittedAt = entity.SubmittedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        private string GetParticipantName(Participant? participant)
        {
            return participant switch
            {
                Person p => $"{p.Name} {p.Surname}",
                Team t => t.Name,
                _ => "Unknown"
            };
        }

        private string? GetTeamName(Participant? participant)
        {
            if (participant is Person p)
            {
                return p.TeamsAsMember?.FirstOrDefault()?.Name ?? p.Teams?.FirstOrDefault()?.Name;
            }
            if (participant is Team t)
            {
                return t.Name;
            }
            return null;
        }

        public async Task<EntryResponseDto> CreateAsync(EntryRequestDto request)
        {
            int participantId = request.ParticipantId ?? 0;

            if (participantId <= 0 && !string.IsNullOrWhiteSpace(request.ParticipantName))
            {
                var name = request.ParticipantName.Trim();
                var surname = request.ParticipantSurname?.Trim() ?? "";
                
                var existingPerson = await _context.Persons
                    .FirstOrDefaultAsync(p => p.Name == name && p.Surname == surname);
                
                if (existingPerson != null)
                {
                    participantId = existingPerson.Id;
                }
                else
                {
                    var newPerson = new Person
                    {
                        Name = name,
                        Surname = surname,
                        Country = "Україна",
                        DateOfBirth = DateTime.Now.AddYears(-10), 
                        Gender = Gender.Female, 
                        Type = "Person"
                    };
                    _context.Persons.Add(newPerson);
                    await _context.SaveChangesAsync();
                    participantId = newPerson.Id;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.TeamName))
            {
                var teamName = request.TeamName.Trim();
                var existingTeam = await _context.Teams.FirstOrDefaultAsync(t => t.Name == teamName);
                
                Team team;
                if (existingTeam != null)
                {
                    team = existingTeam;
                }
                else
                {
                    var coach = await _context.Persons.FirstOrDefaultAsync() 
                                ?? await CreatePlaceholderCoach();
                    
                    team = new Team
                    {
                        Name = teamName,
                        CoachId = coach.Id,
                        Type = "Team"
                    };
                    _context.Teams.Add(team);
                    await _context.SaveChangesAsync();
                }

                var person = await _context.Persons.Include(p => p.TeamsAsMember).FirstOrDefaultAsync(p => p.Id == participantId);
                if (person != null && !person.TeamsAsMember.Any(t => t.Id == team.Id))
                {
                    person.TeamsAsMember.Add(team);
                    await _context.SaveChangesAsync();
                }
            }

            var entity = new Entry
            {
                CompetitionId = request.CompetitionId,
                ParticipantId = participantId,
                DisciplineId = request.DisciplineId,
                CategoryId = request.CategoryId,
                ApplicationStatus = ApplicationStatus.Pending,
                EntryStatus = EntryStatus.Registered,
                SubmittedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            
            return await GetByIdAsync(entity.Id) ?? entity.Adapt<EntryResponseDto>();
        }

        public async Task<EntryResponseDto> CreateAsync(EntryRequestDto request, int userId, bool isAdmin)
        {
            if (isAdmin)
            {
                return await CreateAsync(request);
            }

            var coachPersonId = await GetCoachPersonIdAsync(userId);
            var competition = await _context.Competitions.FindAsync(request.CompetitionId);
            if (competition == null) throw new NotFoundException(nameof(Competition), request.CompetitionId);

            if (competition.Level == CompetitionLevel.International)
            {
                throw new BadRequestException("Тренер не може подавати заявки на міжнародні змагання. Для міжнародних змагань заявки подає адміністратор.");
            }

            if (competition.Status != CompetitionStatus.Planned && competition.Status != CompetitionStatus.RegistrationOpen)
            {
                throw new BadRequestException("Тренер може подавати заявку лише на змагання, які ще не почалися.");
            }

            var participantId = await ResolveCoachParticipantIdAsync(request, coachPersonId.Value);
            if (await _repository.IsDuplicateEntryAsync(request.CompetitionId, participantId, request.DisciplineId))
            {
                throw new BadRequestException("Цей учасник або команда вже зареєстровані на цю дисципліну.");
            }

            var entity = new Entry
            {
                CompetitionId = request.CompetitionId,
                ParticipantId = participantId,
                DisciplineId = request.DisciplineId,
                CategoryId = request.CategoryId,
                ApplicationStatus = ApplicationStatus.Pending,
                EntryStatus = EntryStatus.Registered,
                SubmittedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return await GetByIdAsync(entity.Id) ?? entity.Adapt<EntryResponseDto>();
        }

        private async Task<Person> CreatePlaceholderCoach()
        {
            var coach = new Person { Name = "System", Surname = "Coach", Country = "UA", DateOfBirth = new DateTime(1980, 1, 1), Type = "Person" };
            _context.Persons.Add(coach);
            await _context.SaveChangesAsync();
            return coach;
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

        public async Task ChangeApplicationStatusAsync(int id, ChangeApplicationStatusDto request)
        {
            var entry = await _repository.GetByIdAsync(id);
            if (entry == null) throw new NotFoundException(nameof(Entry), id);

            entry.ApplicationStatus = request.NewStatus;
            _repository.Update(entry);
            await _unitOfWork.CompleteAsync();
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
        public async Task<IEnumerable<EntryResponseDto>> GetByCompetitionIdAsync(int competitionId)
        {
            var entries = await _repository.GetByCompetitionIdAsync(competitionId);
            return entries.Select(e => MapToResponseDto(e));
        }

        public async Task<IEnumerable<EntryResponseDto>> GetByCompetitionIdForUserAsync(int competitionId, int userId, bool isAdmin)
        {
            if (isAdmin)
            {
                return await GetByCompetitionIdAsync(competitionId);
            }

            var coachPersonId = await GetCoachPersonIdAsync(userId);
            var entries = await RestrictToCoach(
                    EntriesWithDetails().Where(e => e.CompetitionId == competitionId),
                    coachPersonId.Value)
                .OrderByDescending(e => e.SubmittedAt)
                .ToListAsync();

            return entries.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<EntryParticipantOptionDto>> GetParticipantOptionsForUserAsync(int userId)
        {
            var coachPersonId = await GetCoachPersonIdAsync(userId);

            var people = await _context.Persons
                .Include(p => p.TeamsAsMember)
                .Where(p => p.MentorId == coachPersonId || p.TeamsAsMember.Any(t => t.CoachId == coachPersonId))
                .AsNoTracking()
                .Select(p => new EntryParticipantOptionDto
                {
                    Id = p.Id,
                    Name = p.Name + " " + p.Surname,
                    Type = "Person"
                })
                .ToListAsync();

            var teams = await _context.Teams
                .Where(t => t.CoachId == coachPersonId)
                .AsNoTracking()
                .Select(t => new EntryParticipantOptionDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Type = "Team"
                })
                .ToListAsync();

            return people.Concat(teams).OrderBy(p => p.Name);
        }

        private IQueryable<Entry> EntriesWithDetails()
        {
            return _context.Entries
                .Include(e => e.Participant)
                .Include(e => (e.Participant as Person).TeamsAsMember)
                .Include(e => e.Competition)
                .Include(e => e.Discipline)
                .Include(e => e.Category)
                .AsNoTracking();
        }

        private IQueryable<Entry> RestrictToCoach(IQueryable<Entry> query, int coachPersonId)
        {
            return query.Where(e =>
                _context.Persons.Any(p => p.Id == e.ParticipantId
                    && (p.MentorId == coachPersonId || p.TeamsAsMember.Any(t => t.CoachId == coachPersonId)))
                || _context.Teams.Any(t => t.Id == e.ParticipantId && t.CoachId == coachPersonId));
        }

        private async Task<int?> GetCoachPersonIdAsync(int userId)
        {
            var personId = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.PersonId)
                .FirstOrDefaultAsync();

            if (!personId.HasValue)
            {
                throw new BadRequestException("До акаунта тренера не прив'язано профіль тренера.");
            }

            return personId;
        }

        private static bool IsEntryOwnedByCoach(Entry entry, int coachPersonId)
        {
            return entry.Participant switch
            {
                Person person => person.MentorId == coachPersonId
                    || (person.TeamsAsMember?.Any(team => team.CoachId == coachPersonId) ?? false),
                Team team => team.CoachId == coachPersonId,
                _ => false
            };
        }

        private async Task<int> ResolveCoachParticipantIdAsync(EntryRequestDto request, int coachPersonId)
        {
            if (request.ParticipantId.HasValue && request.ParticipantId.Value > 0)
            {
                var participantId = request.ParticipantId.Value;
                var isOwnPerson = await _context.Persons
                    .Include(p => p.TeamsAsMember)
                    .AnyAsync(p => p.Id == participantId
                        && (p.MentorId == coachPersonId || p.TeamsAsMember.Any(t => t.CoachId == coachPersonId)));
                var isOwnTeam = await _context.Teams.AnyAsync(t => t.Id == participantId && t.CoachId == coachPersonId);

                if (!isOwnPerson && !isOwnTeam)
                {
                    throw new BadRequestException("Тренер може подавати заявки лише для своїх підопічних або команд.");
                }

                return participantId;
            }

            var fullName = $"{request.ParticipantName} {request.ParticipantSurname}".Trim();
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var name = parts.FirstOrDefault() ?? string.Empty;
                var surname = string.Join(' ', parts.Skip(1));

                var person = await _context.Persons
                    .Include(p => p.TeamsAsMember)
                    .FirstOrDefaultAsync(p => p.Name == name
                        && p.Surname == surname
                        && (p.MentorId == coachPersonId || p.TeamsAsMember.Any(t => t.CoachId == coachPersonId)));

                if (person != null)
                {
                    return person.Id;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.TeamName))
            {
                var teamName = request.TeamName.Trim();
                var team = await _context.Teams.FirstOrDefaultAsync(t => t.Name == teamName && t.CoachId == coachPersonId);
                if (team != null)
                {
                    return team.Id;
                }
            }

            throw new BadRequestException("Оберіть існуючого підопічного або команду, яку ви тренуєте.");
        }
    }
}
