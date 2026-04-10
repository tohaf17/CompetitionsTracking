using CompetitionsTracking.Application.DTOs.Entry;
using CompetitionsTracking.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CompetitionsTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntryController : ControllerBase
    {
        private readonly IEntryService _service;

        public EntryController(IEntryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EntryRequestDto request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EntryRequestDto request)
        {
            await _service.UpdateAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("competition/{competitionId}/controversial")]
        public async Task<IActionResult> GetControversialEntries(int competitionId)
        {
            var result = await _service.GetControversialEntriesAsync(competitionId);
            return Ok(result);
        }
        [HttpPatch("bulk-status")]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateAppStatusDto request)
        {
            var updatedCount = await _service.BulkUpdateAppStatusAsync(request);
            return Ok(new { message = $"Успішно оновлено статус для {updatedCount} заявок." });
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeEntryStatusDto request)
        {
            try
            {
                await _service.ChangeEntryStatusAsync(id, request);
                return NoContent();
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPatch("{id}/disqualify")]
        public async Task<IActionResult> Disqualify(int id)
        {
            try
            {
                await _service.DisqualifyAsync(id);
                return Ok(new { message = "Учасника дискваліфіковано (DNS), результати анульовано." });
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/transfer")]
        public async Task<IActionResult> TransferEntry(int id, [FromBody] TransferEntryDto request)
        {
            try
            {
                await _service.TransferEntryAsync(id, request);
                return Ok(new { message = "Заявку успішно перенесено у нову категорію/дисципліну." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("competition/{competitionId}/start-list")]
        public async Task<IActionResult> GetStartList(int competitionId)
        {
            var result = await _service.GetStartListAsync(competitionId);
            return Ok(result);
        }

        [HttpGet("competition/{competitionId}/missing-scores")]
        public async Task<IActionResult> GetMissingScores(int competitionId, [FromQuery] int expectedCount = 4)
        {
            var result = await _service.GetMissingScoresAsync(competitionId, expectedCount);
            return Ok(result);
        }

        [HttpGet("competition/{competitionId}/analytics")]
        public async Task<IActionResult> GetAnalytics(int competitionId)
        {
            var result = await _service.GetAnalyticsAsync(competitionId);
            return Ok(result);
        }
    }
}
