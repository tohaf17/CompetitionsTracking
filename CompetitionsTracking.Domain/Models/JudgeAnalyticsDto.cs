namespace CompetitionsTracking.Domain.Models
{
    public class JudgeAnalyticsDto
    {
        public int JudgeId { get; set; }
        public string JudgeName { get; set; } = string.Empty;
        public int TotalPerformancesJudged { get; set; }
        public float AverageScoreGiven { get; set; }
        public float AverageScoreDeviation { get; set; }
    }
}
