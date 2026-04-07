namespace CompetitionsTracking.Application.DTOs.Team
{
    public class TeamRequestDto
    {
        public string Type { get; set; } = "Team";
        public string Name { get; set; } = string.Empty;
        public int CoachId { get; set; }
    }

    public class TeamResponseDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = "Team";
        public string Name { get; set; } = string.Empty;
        public int CoachId { get; set; }
    }
}
