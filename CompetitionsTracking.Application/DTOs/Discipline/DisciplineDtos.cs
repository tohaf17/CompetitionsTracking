namespace CompetitionsTracking.Application.DTOs.Discipline
{
    public class DisciplineRequestDto
    {
        public string Type { get; set; } = string.Empty;
        public int ApparatusId { get; set; }
    }

    public class DisciplineResponseDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public int ApparatusId { get; set; }
    }
    public class DisciplineStatsDto
    {
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; } = string.Empty;
        public int TotalEntries { get; set; }
        public int CompetitionsFeaturedIn { get; set; }
        public float AverageScore { get; set; }
    }
}
