using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Application.DTOs.Entry
{
    public class EntryRequestDto
    {
        public int CompetitionId { get; set; }
        public int ParticipantId { get; set; }
        public int DisciplineId { get; set; }
        public int CategoryId { get; set; }
        public CompetitionsTracking.Domain.Entities.ApplicationStatus ApplicationStatus { get; set; }
        public CompetitionsTracking.Domain.Entities.EntryStatus EntryStatus { get; set; }
        public System.DateTime SubmittedAt { get; set; }
        public System.DateTime? UpdatedAt { get; set; }
    }

    public class EntryResponseDto
    {
        public int Id { get; set; }
        public int CompetitionId { get; set; }
        public int ParticipantId { get; set; }
        public int DisciplineId { get; set; }
        public int CategoryId { get; set; }
        public CompetitionsTracking.Domain.Entities.ApplicationStatus ApplicationStatus { get; set; }
        public CompetitionsTracking.Domain.Entities.EntryStatus EntryStatus { get; set; }
        public System.DateTime SubmittedAt { get; set; }
        public System.DateTime? UpdatedAt { get; set; }
    }
    public class BulkUpdateAppStatusDto
    {
        public int CompetitionId { get; set; }
        public int CategoryId { get; set; }
        public ApplicationStatus NewStatus { get; set; }
    }

    public class ChangeEntryStatusDto
    {
        public EntryStatus NewStatus { get; set; }
    }

    public class TransferEntryDto
    {
        public int NewCategoryId { get; set; }
        public int NewDisciplineId { get; set; }
    }

    public class EntryAnalyticsDto
    {
        public int TotalEntries { get; set; }
        public Dictionary<string, int> EntriesByStatus { get; set; } = new();
        public Dictionary<string, int> EntriesByCategory { get; set; } = new();
    }
}
