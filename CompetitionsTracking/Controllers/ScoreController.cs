using CompetitionsTracking.Application.DTOs.Score;
using CompetitionsTracking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompetitionsTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScoreController : ControllerBase
    {
        private readonly IScoreService _service;

        public ScoreController(IScoreService service)
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
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> Create([FromBody] ScoreRequestDto request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> Update(int id, [FromBody] ScoreRequestDto request)
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

        [HttpGet("competition/{competitionId}/anomalies")]
        public async Task<IActionResult> GetScoreAnomalies(int competitionId)
        {
            var result = await _service.GetScoreAnomaliesAsync(competitionId);
            return Ok(result);
        }
        [HttpGet("entry/{entryId}")]
        public async Task<IActionResult> GetScoresByEntry(int entryId)
        {
            var result = await _service.GetScoresByEntryAsync(entryId);
            if (!result.Any()) return NotFound(new { message = "Оцінок для цього виступу не знайдено." });
            return Ok(result);
        }

        [HttpGet("entry/{entryId}/breakdown")]
        public async Task<IActionResult> GetEntryScoreBreakdown(int entryId)
        {
            var result = await _service.GetEntryScoreBreakdownAsync(entryId);
            if (result == null) return NotFound(new { message = "Неможливо сформувати розрахунок, оцінки відсутні." });
            return Ok(result);
        }
    }
}
