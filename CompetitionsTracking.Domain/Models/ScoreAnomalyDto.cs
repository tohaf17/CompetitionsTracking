namespace CompetitionsTracking.Domain.Models
{
    public class ScoreAnomalyDto
    {
        public int ScoreId { get; set; }
        public string JudgeName { get; set; } = string.Empty;
        public int EntryId { get; set; }
        public float ScoreValue { get; set; }
        public float AverageEntryScore { get; set; }
        public float Deviation { get; set; }
    }
}
