namespace CompetitionsTracking.Domain.Models
{
    public record ParticipantPerformanceDto
    {
        public int CompetitionId { get; init; }
        public string CompetitionName { get; init; } = string.Empty;
        public string ApparatusName { get; init; } = string.Empty;
        public float FinalScore { get; init; }
        public int Placement { get; init; }
        public DateTime CompetitionDate { get; init; }
    }
}
