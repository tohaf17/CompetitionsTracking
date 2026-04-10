namespace CompetitionsTracking.Application.DTOs.Person
{
    public class PersonRequestDto
    {
        public string Type { get; set; } = "Person";
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public System.DateTime DateOfBirth { get; set; }
        public int? MentorId { get; set; }
        public CompetitionsTracking.Domain.Entities.Gender Gender { get; set; }
    }

    public class PersonResponseDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = "Person";
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public System.DateTime DateOfBirth { get; set; }
        public int? MentorId { get; set; }
        public CompetitionsTracking.Domain.Entities.Gender Gender { get; set; }
    }
    public class MenteeSummaryDto
    {
        public int PersonId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class TeamAffiliationDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Coach" або "Member"
    }
}
