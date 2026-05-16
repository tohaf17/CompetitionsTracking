namespace CompetitionsTracking.Domain.Models
{
    public record TeamMedalTallyDto
    {
        public int TeamId { get; init; }
        public string TeamName { get; init; } = string.Empty;
        public int GoldMedals { get; init; }
        public int SilverMedals { get; init; }
        public int BronzeMedals { get; init; }
        public int TotalMedals { get; init; }
    }
}
