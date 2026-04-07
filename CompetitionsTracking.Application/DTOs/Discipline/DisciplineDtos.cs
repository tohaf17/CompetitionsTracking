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
}
