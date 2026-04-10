using System;
using System.Collections.Generic;
using System.Text;

namespace CompetitionsTracking.Application.DTOs.Category
{
    public record CategoryRequestDto
    {
        public string Type { get; init; } = string.Empty;
        public int? MinAge { get; init; }
        public int? MaxAge { get; init; }
    }
}
