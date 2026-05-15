using CompetitionsTracking.Application.DTOs.Common;
using CompetitionsTracking.Application.DTOs.Entry;
using CompetitionsTracking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EntryController : ControllerBase
    {
        private readonly IEntryService _service;

        public EntryController(IEntryService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Trainee,Guest")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
        {
            var result = await _service.GetAllForUserAsync(pagination, CurrentUserId(), CurrentUserRole());
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Trainee,Guest")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> Create([FromBody] EntryRequestDto request)
        {
            var result = await _service.CreateAsync(request, CurrentUserId(), CurrentUserRole());
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> Update(int id, [FromBody] EntryRequestDto request)
        {
            await _service.UpdateAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("competition/{competitionId}/controversial")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetControversialEntries(int competitionId)
        {
            var result = await _service.GetControversialEntriesAsync(competitionId);
            return Ok(result);
        }

        [HttpPatch("bulk-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateAppStatusDto request)
        {
            var updatedCount = await _service.BulkUpdateAppStatusAsync(request);
            return Ok(new { message = $"Успішно оновлено статус для {updatedCount} заявок." });
        }

        [HttpPatch("{id}/application-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeApplicationStatus(int id, [FromBody] ChangeApplicationStatusDto request)
        {
            await _service.ChangeApplicationStatusAsync(id, request);
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeEntryStatusDto request)
        {
            await _service.ChangeEntryStatusAsync(id, request);
            return NoContent();
        }

        [HttpPatch("{id}/disqualify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Disqualify(int id)
        {
            await _service.DisqualifyAsync(id);
            return Ok(new { message = "Учасника дискваліфіковано (DNS), результати анульовано." });
        }

        [HttpPost("{id}/transfer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TransferEntry(int id, [FromBody] TransferEntryDto request)
        {
            await _service.TransferEntryAsync(id, request);
            return Ok(new { message = "Заявку успішно перенесено у нову категорію/дисципліну." });
        }

        [HttpGet("competition/{competitionId}/start-list")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStartList(int competitionId)
        {
            var result = await _service.GetStartListAsync(competitionId);
            return Ok(result);
        }

        [HttpGet("competition/{competitionId}/missing-scores")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMissingScores(int competitionId, [FromQuery] int expectedCount = 4)
        {
            var result = await _service.GetMissingScoresAsync(competitionId, expectedCount);
            return Ok(result);
        }

        [HttpGet("competition/{competitionId}/analytics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAnalytics(int competitionId)
        {
            var result = await _service.GetAnalyticsAsync(competitionId);
            return Ok(result);
        }
        [HttpGet("competition/{competitionId}")]
        [Authorize(Roles = "Admin,Trainee,Guest")]
        public async Task<IActionResult> GetByCompetitionId(int competitionId)
        {
            var result = await _service.GetByCompetitionIdForUserAsync(competitionId, CurrentUserId(), CurrentUserRole());
            return Ok(result);
        }

        [HttpGet("my-participants")]
        [Authorize(Roles = "Trainee,Guest")]
        public async Task<IActionResult> GetMyParticipantOptions()
        {
            var result = await _service.GetParticipantOptionsForUserAsync(CurrentUserId(), CurrentUserRole());
            return Ok(result);
        }

        private int CurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private UserRole CurrentUserRole()
        {
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            return roleClaim != null ? Enum.Parse<UserRole>(roleClaim) : UserRole.Guest;
        }
    }
}
