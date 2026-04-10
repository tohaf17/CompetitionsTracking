namespace CompetitionsTracking.Application.DTOs.Discipline
{
    public record DisciplineRequestDto
    {
        public string Type { get; init; } = string.Empty;
        public int ApparatusId { get; init; }
    }

    public record DisciplineResponseDto
    {
        public int Id { get; init; }
        public string Type { get; init; } = string.Empty;
        public int ApparatusId { get; init; }
    }
    public record DisciplineStatsDto
    {
        public int DisciplineId { get; init; }
        public string DisciplineName { get; init; } = string.Empty;
        public int TotalEntries { get; init; }
        public int CompetitionsFeaturedIn { get; init; }
        public float AverageScore { get; init; }
    }
}
