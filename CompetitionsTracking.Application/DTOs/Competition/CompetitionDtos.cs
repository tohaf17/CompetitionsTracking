using CompetitionsTracking.Domain.Entities;


namespace CompetitionsTracking.Application.DTOs.Competition
{
    public record CompetitionRequestDto
    {
        public string Title { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public CompetitionStatus Status { get; init; }
    }

    public record CompetitionResponseDto
    {
        public int Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public CompetitionStatus Status { get; init; }
    }
    public record ChangeCompetitionStatusDto
    {
        public CompetitionStatus NewStatus { get; init; }
    }

    public record CompetitionFilterDto
    {
        public CompetitionStatus? Status { get; init; }
        public string? City { get; init; }
    }

    public record CompetitionSummaryDto
    {
        public int CompetitionId { get; init; }
        public int TotalEntries { get; init; }
        public int PendingEntries { get; init; }
        public int AcceptedEntries { get; init; }
        public int UniqueDisciplinesCount { get; init; }
    }
}
