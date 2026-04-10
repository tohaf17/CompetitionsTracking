namespace CompetitionsTracking.Domain.Models
{
    public class TeamDominanceMetricDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int TotalParticipants { get; set; }
        public float CumulativePoints { get; set; }
        public float AveragePointsPerParticipant { get; set; }
    }
}
