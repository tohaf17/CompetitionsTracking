namespace CompetitionsTracking.Application.DTOs.Apparatus
{
    public class ApparatusRequestDto
    {
        public string Type { get; set; } = string.Empty;
    }

    public class ApparatusResponseDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
