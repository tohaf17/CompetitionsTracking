using CompetitionsTracking.Application.DTOs.Common;
using CompetitionsTracking.Application.DTOs.Competition;
using CompetitionsTracking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompetitionsTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class CompetitionController : ControllerBase
    {
        private readonly ICompetitionService _service;

        public CompetitionController(ICompetitionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CompetitionFilterDto filter, [FromQuery] PaginationParams pagination)
        {
            var result = await _service.GetAllAsync(filter, pagination);
            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeCompetitionStatusDto request)
        {
            await _service.ChangeStatusAsync(id, request);
            return NoContent();
        }

        [HttpPost("{id}/award-medals")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AwardMedals(int id)
        {
            await _service.AwardMedalsAsync(id);
            return Ok(new { message = "Медалі успішно розподілені між учасниками." });
        }

        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetSummary(int id)
        {
            var result = await _service.GetSummaryAsync(id);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CompetitionRequestDto request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CompetitionRequestDto request)
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

        [HttpGet("{id}/leaderboard")]
        public async Task<IActionResult> GetCompetitionLeaderboard(int id)
        {
            var result = await _service.GetCompetitionLeaderboardAsync(id);
            return Ok(result);
        }
    }
}
