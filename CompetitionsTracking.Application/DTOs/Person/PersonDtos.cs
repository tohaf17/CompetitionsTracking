using CompetitionsTracking.Domain.Entities;
namespace CompetitionsTracking.Application.DTOs.Person
{
    public record PersonRequestDto
    {
        public string Type { get; init; } = "Person";
        public string Name { get; init; } = string.Empty;
        public string Surname { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public DateTime DateOfBirth { get; init; }
        public int? MentorId { get; init; }
        public Gender Gender { get; init; }
    }

    public record PersonResponseDto
    {
        public int Id { get; init; }
        public string Type { get; init; } = "Person";
        public string Name { get; init; } = string.Empty;
        public string Surname { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public DateTime DateOfBirth { get; init; }
        public int? MentorId { get; init; }
        public Gender Gender { get; init; }
    }
    public record MenteeSummaryDto
    {
        public int PersonId { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
    }

    public record TeamAffiliationDto
    {
        public int TeamId { get; init; }
        public string TeamName { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty; 
    }
}
