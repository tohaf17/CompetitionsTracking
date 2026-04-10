namespace CompetitionsTracking.Application.DTOs.Judge
{
    public class JudgeRequestDto
    {
        public int PersonId { get; set; }
        public string QualificationLevel { get; set; } = string.Empty;
    }

    public class JudgeResponseDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string QualificationLevel { get; set; } = string.Empty;
    }
    public class PendingEvaluationDto
    {
        public int EntryId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string DisciplineName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }

    public class JudgeWorkloadDto
    {
        public string DisciplineName { get; set; } = string.Empty;
        public int ScoresGiven { get; set; }
    }

    public class ConflictOfInterestDto
    {
        public int ScoreId { get; set; }
        public int EntryId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string SharedAttribute { get; set; } = string.Empty;
        public float GivenScore { get; set; } // Змінено на float
        public string ScoreType { get; set; } = string.Empty; // Додано тип оцінки
    }

    public class JudgeScoreHistoryDto
    {
        public int ScoreId { get; set; }
        public int EntryId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public float ScoreValue { get; set; } // Змінено на float
        public string ScoreType { get; set; } = string.Empty; // Додано тип оцінки
        // Поле CreatedAt видалено
    }
}
