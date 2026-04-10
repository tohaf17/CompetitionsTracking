namespace CompetitionsTracking.Domain.Models
{
    public class ParticipantPerformanceDto
    {
        public int CompetitionId { get; set; }
        public string CompetitionName { get; set; } = string.Empty;
        public string ApparatusName { get; set; } = string.Empty;
        public float FinalScore { get; set; }
        public int Placement { get; set; }
        public DateTime CompetitionDate { get; set; }
    }
}
