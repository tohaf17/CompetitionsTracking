namespace CompetitionsTracking.Application.DTOs.Competition
{
    public class CompetitionRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public CompetitionsTracking.Domain.Entities.CompetitionStatus Status { get; set; }
    }

    public class CompetitionResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public CompetitionsTracking.Domain.Entities.CompetitionStatus Status { get; set; }
    }
}
