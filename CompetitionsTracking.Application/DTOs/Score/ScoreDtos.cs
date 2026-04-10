using CompetitionsTracking.Domain.Entities;
namespace CompetitionsTracking.Application.DTOs.Score
{
    public record ScoreRequestDto
    {
        public int EntryId { get; init; }
        public int JudgeId { get; init; }
        public ScoreType Type { get; init; }
        public float ScoreValue { get; init; }
    }

    public record ScoreResponseDto
    {
        public int Id { get; init; }
        public int EntryId { get; init; }
        public int JudgeId { get; init; }
        public ScoreType Type { get; init; }
        public float ScoreValue { get; init; }
    }
    public record EntryScoreDetailDto
    {
        public int ScoreId { get; init; }
        public string JudgeName { get; init; } = string.Empty;
        public string ScoreType { get; init; } = string.Empty;
        public float ScoreValue { get; init; }
    }

    public record EntryScoreBreakdownDto
    {
        public int EntryId { get; init; }
        public float TotalDifficulty { get; init; } 
        public float AverageExecution { get; init; } 
        public float TotalPenalties { get; init; }
        public float CalculatedTotalScore { get; init; }
    }
}
