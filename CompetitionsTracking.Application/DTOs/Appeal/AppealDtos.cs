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

    public class ApproveAppealRequestDto
    {
        public int ScoreIdToEdit { get; set; }
        public float NewScoreValue { get; set; }
    }

    public class PendingAppealDto
    {
        public int Id { get; set; }
        public int ResultId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public CompetitionsTracking.Domain.Entities.AppealStatus Status { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public int CompetitionId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
    }

    public class AppealDossierDto
    {
        public int AppealId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public CompetitionsTracking.Domain.Entities.AppealStatus Status { get; set; }
        public float FinalScore { get; set; }
        public System.Collections.Generic.List<DossierScoreDto> Scores { get; set; } = new();
    }

    public class DossierScoreDto
    {
        public int ScoreId { get; set; }
        public float Value { get; set; }
        public string ScoreType { get; set; } = string.Empty;
        public string JudgeName { get; set; } = string.Empty;
    }
}
