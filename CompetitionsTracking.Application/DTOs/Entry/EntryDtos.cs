using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Application.DTOs.Entry
{
    public record EntryRequestDto
    {
        public int CompetitionId { get; init; }
        public int ParticipantId { get; init; }
        public int DisciplineId { get; init; }
        public int CategoryId { get; init; }
        public ApplicationStatus ApplicationStatus { get; init; }
        public EntryStatus EntryStatus { get; init; }
        public DateTime SubmittedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }

    public record EntryResponseDto
    {
        public int Id { get; init; }
        public int CompetitionId { get; init; }
        public int ParticipantId { get; init; }
        public int DisciplineId { get; init; }
        public int CategoryId { get; init; }
        public ApplicationStatus ApplicationStatus { get; init; }
        public EntryStatus EntryStatus { get; init; }
        public DateTime SubmittedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
    public record BulkUpdateAppStatusDto
    {
        public int CompetitionId { get; init; }
        public int CategoryId { get; init; }
        public ApplicationStatus NewStatus { get; init; }
    }

    public record ChangeEntryStatusDto
    {
        public EntryStatus NewStatus { get; init; }
    }

    public record TransferEntryDto
    {
        public int NewCategoryId { get; init; }
        public int NewDisciplineId { get; init; }
    }

    public record EntryAnalyticsDto
    {
        public int TotalEntries { get; init; }
        public Dictionary<string, int> EntriesByStatus { get; init; } = new();
        public Dictionary<string, int> EntriesByCategory { get; init; } = new();
    }
}
