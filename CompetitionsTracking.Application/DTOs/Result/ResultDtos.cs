namespace CompetitionsTracking.Application.DTOs.Result
{
    public class ResultRequestDto
    {
        public int EntryId { get; set; }
        public int Place { get; set; }
        public float FinalScore { get; set; }
        public string AwardedMedal { get; set; }
    }

    public class ResultResponseDto
    {
        public int Id { get; set; }
        public int EntryId { get; set; }
        public int Place { get; set; }
        public float FinalScore { get; set; }
        public string AwardedMedal { get; set; }
    }
}
