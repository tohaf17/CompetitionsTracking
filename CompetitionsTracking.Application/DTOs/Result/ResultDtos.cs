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
    public class LeaderboardEntryDto
    {
        public int Place { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public float FinalScore { get; set; }
    }

    public class CountryMedalTallyDto
    {
        public string Country { get; set; } = string.Empty;
        public int Gold { get; set; }
        public int Silver { get; set; }
        public int Bronze { get; set; }
        public int TotalMedals => Gold + Silver + Bronze;
    }

    public class DisciplineRecordDto
    {
        public string ParticipantName { get; set; } = string.Empty;
        public string CompetitionName { get; set; } = string.Empty;
        public float FinalScore { get; set; }
        public System.DateTime CompetitionDate { get; set; }
    }
}
