namespace CompetitionsTracking.Domain.Models
{
    public record ControversialEntryDto
    {
        public int EntryId { get; init; }
        public string ParticipantName { get; init; } = string.Empty;
        public string CompetitionName { get; init; } = string.Empty;
        public float HighestScore { get; init; }
        public float LowestScore { get; init; }
        public float ScoreGap { get; init; }
    }
}
