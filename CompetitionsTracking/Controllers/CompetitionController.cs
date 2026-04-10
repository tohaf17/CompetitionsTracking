using CompetitionsTracking.Application.DTOs.Competition;
using CompetitionsTracking.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CompetitionsTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompetitionController : ControllerBase
    {
        private readonly ICompetitionService _service;

        public CompetitionController(ICompetitionService service)
        {
            _service = service;
        }

        // Онови існуючий [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CompetitionFilterDto filter)
        {
            // Якщо фільтри не передані, об'єкт filter матиме всі властивості як null
            var result = await _service.GetAllAsync(filter);
            return Ok(result);
        }

        // Додай нові ендпоінти
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeCompetitionStatusDto request)
        {
            try
            {
                await _service.ChangeStatusAsync(id, request);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/award-medals")]
        public async Task<IActionResult> AwardMedals(int id)
        {
            try
            {
                await _service.AwardMedalsAsync(id);
                return Ok(new { message = "Медалі успішно розподілені між учасниками." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetSummary(int id)
        {
            var result = await _service.GetSummaryAsync(id);
            if (result == null) return NotFound();
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
        public async Task<IActionResult> Create([FromBody] CompetitionRequestDto request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CompetitionRequestDto request)
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

        [HttpGet("{id}/leaderboard")]
        public async Task<IActionResult> GetCompetitionLeaderboard(int id)
        {
            var result = await _service.GetCompetitionLeaderboardAsync(id);
            return Ok(result);
        }
    }
}
