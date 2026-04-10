namespace CompetitionsTracking.Application.DTOs.Category
{
    public class CategoryRequestDto
    {
        public string Type { get; set; } = string.Empty;
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
    }

    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
    }
    public class CategoryStatsDto
    {
        public int CategoryId { get; set; }
        public string CategoryType { get; set; } = string.Empty;
        public int TotalEntries { get; set; }
        public int CompetitionsFeaturedIn { get; set; }
        public float AverageScore { get; set; }
    }
}
