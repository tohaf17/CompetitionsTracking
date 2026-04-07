namespace CompetitionsTracking.Application.DTOs.Appeal
{
    public class AppealRequestDto
    {
        public int ResultId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public CompetitionsTracking.Domain.Entities.AppealStatus Status { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime ResolvedAt { get; set; }
    }

    public class AppealResponseDto
    {
        public int Id { get; set; }
        public int ResultId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public CompetitionsTracking.Domain.Entities.AppealStatus Status { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime ResolvedAt { get; set; }
    }
}
