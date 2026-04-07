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
}
