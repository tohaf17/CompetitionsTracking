
namespace CompetitionsTracking.Application.DTOs.Team
{
    public record TeamRequestDto
    {
        public string Type { get; init; } = "Team";
        public string Name { get; init; } = string.Empty;
        public int? CoachId { get; init; }
        public string? NewCoachName { get; init; }
        public string? NewCoachSurname { get; init; }
    }

    public record TeamResponseDto
    {
        public int Id { get; init; }
        public string Type { get; init; } = "Team";
        public string Name { get; init; } = string.Empty;
        public int CoachId { get; init; }
        public string CoachFullName { get; init; } = string.Empty;
        public int GoldMedals { get; init; }
        public int SilverMedals { get; init; }
        public int BronzeMedals { get; init; }
        public int TotalMedals { get; init; }
        public List<TeamMemberDto> Members { get; init; } = new List<TeamMemberDto>();
    }
    public record TeamRosterDto
    {
        public int TeamId { get; init; }
        public string TeamName { get; init; } = string.Empty;
        public string CoachFullName { get; init; } = string.Empty;
        public List<TeamMemberDto> Members { get; init; } = new List<TeamMemberDto>();
    }

    public record TeamMemberDto
    {
        public int PersonId { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
    }
}
