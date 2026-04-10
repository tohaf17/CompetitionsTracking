using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Application.DTOs.Competition
{
    public class CompetitionRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public CompetitionsTracking.Domain.Entities.CompetitionStatus Status { get; set; }
    }

    public class CompetitionResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public CompetitionsTracking.Domain.Entities.CompetitionStatus Status { get; set; }
    }
    public class ChangeCompetitionStatusDto
    {
        public CompetitionStatus NewStatus { get; set; }
    }

    public class CompetitionFilterDto
    {
        public CompetitionStatus? Status { get; set; }
        public string? City { get; set; }
    }

    public class CompetitionSummaryDto
    {
        public int CompetitionId { get; set; }
        public int TotalEntries { get; set; }
        public int PendingEntries { get; set; }
        public int AcceptedEntries { get; set; }
        public int UniqueDisciplinesCount { get; set; }
    }
}
