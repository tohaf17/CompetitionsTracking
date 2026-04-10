using System;
using System.Collections.Generic;
using System.Text;

namespace CompetitionsTracking.Application.DTOs.Appeal
{
    public record DossierScoreDto
    {
        public int ScoreId { get; init; }
        public float Value { get; init; }
        public string ScoreType { get; init; } = string.Empty;
        public string JudgeName { get; init; } = string.Empty;
    }
}
