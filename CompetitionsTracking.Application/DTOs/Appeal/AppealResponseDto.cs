using System;
using System.Collections.Generic;
using System.Text;
using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Application.DTOs.Appeal
{
    public record AppealResponseDto
    {
        public int Id { get; init; }
        public int ResultId { get; init; }
        public string Reason { get; init; } = string.Empty;
        public AppealStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime ResolvedAt { get; init; }
    }
}
