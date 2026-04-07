namespace CompetitionsTracking.Application.DTOs.Score
{
    public class ScoreRequestDto
    {
        public int EntryId { get; set; }
        public int JudgeId { get; set; }
        public CompetitionsTracking.Domain.Entities.ScoreType Type { get; set; }
        public float ScoreValue { get; set; }
    }

    public class ScoreResponseDto
    {
        public int Id { get; set; }
        public int EntryId { get; set; }
        public int JudgeId { get; set; }
        public CompetitionsTracking.Domain.Entities.ScoreType Type { get; set; }
        public float ScoreValue { get; set; }
    }
}
