using CompetitionsTracking.Application.DTOs.Judge;
using CompetitionsTracking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompetitionsTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JudgeController : ControllerBase
    {
        private readonly IJudgeService _service;

        public JudgeController(IJudgeService service)
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
        public async Task<IActionResult> Create([FromBody] JudgeRequestDto request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> Update(int id, [FromBody] JudgeRequestDto request)
        {
            await _service.UpdateAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/analytics")]
        public async Task<IActionResult> GetJudgeAnalytics(int id)
        {
            var result = await _service.GetJudgeAnalyticsAsync(id);
            return Ok(result);
        }

        [HttpGet("{id}/competitions/{competitionId}/pending")]
        public async Task<IActionResult> GetPendingEvaluations(int id, int competitionId)
        {
            var result = await _service.GetPendingEvaluationsAsync(id, competitionId);
            return Ok(result);
        }

        [HttpGet("{id}/conflicts")]
        public async Task<IActionResult> GetConflictsOfInterest(int id)
        {
            var result = await _service.GetConflictsOfInterestAsync(id);
            if (!result.Any())
            {
                return Ok(new { message = "Конфліктів інтересів не виявлено. Суддя може продовжувати роботу." });
            }
            return Ok(result);
        }

        [HttpGet("{id}/competitions/{competitionId}/workload")]
        public async Task<IActionResult> GetWorkloadSummary(int id, int competitionId)
        {
            var result = await _service.GetWorkloadSummaryAsync(id, competitionId);
            return Ok(result);
        }

        [HttpGet("{id}/competitions/{competitionId}/scores")]
        public async Task<IActionResult> GetJudgeScores(int id, int competitionId)
        {
            var result = await _service.GetJudgeScoresInCompetitionAsync(id, competitionId);
            return Ok(result);
        }
    }
}
