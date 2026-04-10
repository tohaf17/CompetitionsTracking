namespace CompetitionsTracking.Domain.Models
{
    public class TeamMedalTallyDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int GoldMedals { get; set; }
        public int SilverMedals { get; set; }
        public int BronzeMedals { get; set; }
        public int TotalMedals { get; set; }
    }
}
