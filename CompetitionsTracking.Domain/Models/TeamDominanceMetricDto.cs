namespace CompetitionsTracking.Domain.Models
{
    public record TeamDominanceMetricDto
    {
        public int TeamId { get; init; }
        public string TeamName { get; init; } = string.Empty;
        public int TotalParticipants { get; init; }
        public double CumulativePoints { get; init; }
        public double AveragePointsPerParticipant { get; init; }
    }
}
