namespace CompetitionsTracking.Domain.Models
{
    public record JudgeAnalyticsDto
    {
        public int JudgeId { get; init; }
        public string JudgeName { get; init; } = string.Empty;
        public int TotalPerformancesJudged { get; init; }
        public float AverageScoreGiven { get; init; }
        public float AverageScoreDeviation { get; init; }
    }
}
