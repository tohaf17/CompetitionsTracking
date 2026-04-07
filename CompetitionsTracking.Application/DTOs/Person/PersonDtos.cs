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
}
