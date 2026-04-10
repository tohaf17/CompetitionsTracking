namespace CompetitionsTracking.Domain.Models
{
    public class ControversialEntryDto
    {
        public int EntryId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string CompetitionName { get; set; } = string.Empty;
        public float HighestScore { get; set; }
        public float LowestScore { get; set; }
        public float ScoreGap { get; set; }
    }
}
