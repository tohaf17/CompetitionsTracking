namespace CompetitionsTracking.Domain.Models
{
    public record ScoreAnomalyDto
    {
        public int ScoreId { get; init; }
        public string ParticipantName { get; init; } = string.Empty;
        public string JudgeName { get; init; } = string.Empty;
        public int EntryId { get; init; }
        public string ScoreType { get; init; } = string.Empty;
        public float ScoreValue { get; init; }
        public float AverageEntryScore { get; init; }
        public float Deviation { get; init; }
    }
}
