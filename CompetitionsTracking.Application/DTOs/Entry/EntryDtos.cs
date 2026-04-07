namespace CompetitionsTracking.Application.DTOs.Entry
{
    public class EntryRequestDto
    {
        public int CompetitionId { get; set; }
        public int ParticipantId { get; set; }
        public int DisciplineId { get; set; }
        public int CategoryId { get; set; }
        public CompetitionsTracking.Domain.Entities.ApplicationStatus ApplicationStatus { get; set; }
        public CompetitionsTracking.Domain.Entities.EntryStatus EntryStatus { get; set; }
        public System.DateTime SubmittedAt { get; set; }
        public System.DateTime? UpdatedAt { get; set; }
    }

    public class EntryResponseDto
    {
        public int Id { get; set; }
        public int CompetitionId { get; set; }
        public int ParticipantId { get; set; }
        public int DisciplineId { get; set; }
        public int CategoryId { get; set; }
        public CompetitionsTracking.Domain.Entities.ApplicationStatus ApplicationStatus { get; set; }
        public CompetitionsTracking.Domain.Entities.EntryStatus EntryStatus { get; set; }
        public System.DateTime SubmittedAt { get; set; }
        public System.DateTime? UpdatedAt { get; set; }
    }
}
