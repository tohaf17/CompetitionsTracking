namespace CompetitionsTracking.Domain.Models
{
    public class LeaderboardDto
    {
        public int ParticipantId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string DisciplineName { get; set; } = string.Empty;
        public float TotalScore { get; set; }
        public int CalculatedRank { get; set; }
    }
}
