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
    public class EntryScoreDetailDto
    {
        public int ScoreId { get; set; }
        public string JudgeName { get; set; } = string.Empty;
        public string ScoreType { get; set; } = string.Empty;
        public float ScoreValue { get; set; }
    }

    public class EntryScoreBreakdownDto
    {
        public int EntryId { get; set; }
        public float TotalDifficulty { get; set; } // Сума оцінок D
        public float AverageExecution { get; set; } // Середня оцінка E (чи A)
        public float TotalPenalties { get; set; } // Сума штрафів
        public float CalculatedTotalScore { get; set; } // Орієнтовний фінальний бал
    }
}
