namespace CompetitionsTracking.Domain.Models
{
    public record LeaderboardDto
    {
        public int ParticipantId { get; init; }
        public string ParticipantName { get; init; } = string.Empty;
        public string CategoryName { get; init; } = string.Empty;
        public string DisciplineName { get; init; } = string.Empty;
        public float TotalScore { get; init; }
        public int CalculatedRank { get; init; }
    }
}
