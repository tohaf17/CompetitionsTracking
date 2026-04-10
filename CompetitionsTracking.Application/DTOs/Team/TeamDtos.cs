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
    public class TeamRosterDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string CoachFullName { get; set; } = string.Empty;
        public List<TeamMemberDto> Members { get; set; } = new List<TeamMemberDto>();
    }

    public class TeamMemberDto
    {
        public int PersonId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
