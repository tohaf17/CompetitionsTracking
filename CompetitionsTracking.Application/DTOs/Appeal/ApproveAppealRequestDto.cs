using System;
using System.Collections.Generic;
using System.Text;

namespace CompetitionsTracking.Application.DTOs.Appeal
{
    public record ApproveAppealRequestDto(int ScoreIdToEdit, float NewScoreValue);
}
