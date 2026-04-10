namespace CompetitionsTracking.Application.DTOs.Judge
{
    public record JudgeRequestDto
    {
        public int PersonId { get; init; }
        public string QualificationLevel { get; init; } = string.Empty;
    }

    public record JudgeResponseDto
    {
        public int Id { get; init; }
        public int PersonId { get; init; }
        public string QualificationLevel { get; init; } = string.Empty;
    }
    public record PendingEvaluationDto
    {
        public int EntryId { get; init; }
        public string ParticipantName { get; init; } = string.Empty;
        public string DisciplineName { get; init; } = string.Empty;
        public string CategoryName { get; init; } = string.Empty;
    }

    public record JudgeWorkloadDto
    {
        public string DisciplineName { get; init; } = string.Empty;
        public int ScoresGiven { get; init; }
    }

    public record ConflictOfInterestDto
    {
        public int ScoreId { get; init; }
        public int EntryId { get; init; }
        public string ParticipantName { get; init; } = string.Empty;
        public string SharedAttribute { get; init; } = string.Empty;
        public float GivenScore { get; init; }
        public string ScoreType { get; init; } = string.Empty;
    }

    public record JudgeScoreHistoryDto
    {
        public int ScoreId { get; init; }
        public int EntryId { get; init; }
        public string ParticipantName { get; init; } = string.Empty;
        public float ScoreValue { get; init; } 
        public string ScoreType { get; init; } = string.Empty;
        
    }
}
