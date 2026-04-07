namespace CompetitionsTracking.Application.DTOs.Judge
{
    public class JudgeRequestDto
    {
        public int PersonId { get; set; }
        public string QualificationLevel { get; set; } = string.Empty;
    }

    public class JudgeResponseDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string QualificationLevel { get; set; } = string.Empty;
    }
}
