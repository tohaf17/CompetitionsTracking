using System;
using System.Collections.Generic;
using System.Text;

namespace CompetitionsTracking.Application.DTOs.Category
{
    public record CategoryStatsDto
    {
        public int CategoryId { get; init; }
        public string CategoryType { get; init; } = string.Empty;
        public int TotalEntries { get; init; }
        public int CompetitionsFeaturedIn { get; init; }
        public float AverageScore { get; init; }
    }
}
