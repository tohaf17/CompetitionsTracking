using System;
using System.Collections.Generic;
using System.Text;

namespace CompetitionsTracking.Application.DTOs.Category
{
    public record CategoryResponseDto
    {
        public int Id { get; init; }
        public string Type { get; init; } = string.Empty;
        public int? MinAge { get; init; }
        public int? MaxAge { get; init; }
    }
}
