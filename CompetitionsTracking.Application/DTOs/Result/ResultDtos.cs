using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Application.DTOs.Result
{
    public record ResultRequestDto
    {
        public int EntryId { get; init; }
        public int Place { get; init; }
        public float FinalScore { get; init; }
        public string AwardedMedal { get; init; } = string.Empty;
    }

    public record ResultResponseDto
    {
        public int Id { get; init; }
        public int EntryId { get; init; }
        public int Place { get; init; }
        public float FinalScore { get; init; }
        public string AwardedMedal { get; init; } = string.Empty;
        public string ParticipantName { get; init; } = string.Empty;
        public int CompetitionId { get; init; }
        public string CompetitionName { get; init; } = string.Empty;
        public CompetitionStatus CompetitionStatus { get; init; }
        public CompetitionLevel CompetitionLevel { get; init; }
    }
    public record LeaderboardEntryDto
    {
        public int EntryId { get; init; }
        public int ParticipantId { get; init; }
        public string ParticipantType { get; init; } = string.Empty;
        public int Place { get; init; }
        public string ParticipantName { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public string DisciplineName { get; init; } = string.Empty;
        public string CategoryName { get; init; } = string.Empty;
        public float FinalScore { get; init; }
        public string AwardedMedal { get; init; } = string.Empty;
    }

    public record CountryMedalTallyDto
    {
        public string Country { get; init; } = string.Empty;
        public int Gold { get; init; }
        public int Silver { get; init; }
        public int Bronze { get; init; }
        public int TotalMedals => Gold + Silver + Bronze;
    }

    public record DisciplineRecordDto
    {
        public string ParticipantName { get; init; } = string.Empty;
        public string CompetitionName { get; init; } = string.Empty;
        public float FinalScore { get; init; }
        public DateTime CompetitionDate { get; init; }
    }
}
