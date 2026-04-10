using System;
using System.Collections.Generic;
using CompetitionsTracking.Domain.Entities;
using System.Text;

namespace CompetitionsTracking.Application.DTOs.Appeal
{
    public record AppealDossierDto
    {
        public int AppealId { get; init; }
        public string Reason { get; init; } = string.Empty;
        public AppealStatus Status { get; init; }
        public float FinalScore { get; init; }
        public List<DossierScoreDto> Scores { get; init; } = new();
    }
}
