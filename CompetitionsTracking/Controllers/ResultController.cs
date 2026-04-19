using CompetitionsTracking.Application.DTOs.Result;
using CompetitionsTracking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompetitionsTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResultController : ControllerBase
    {
        private readonly IResultService _service;

        public ResultController(IResultService service)
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
        public async Task<IActionResult> Create([FromBody] ResultRequestDto request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> Update(int id, [FromBody] ResultRequestDto request)
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

        [HttpGet("competition/{competitionId}/team-tally")]
        public async Task<IActionResult> GetTeamMedalTally(int competitionId)
        {
            var result = await _service.GetTeamMedalTallyAsync(competitionId);
            return Ok(result);
        }
        [HttpGet("competition/{competitionId}/leaderboard")]
        public async Task<IActionResult> GetLeaderboard(int competitionId, [FromQuery] int? disciplineId, [FromQuery] int? categoryId)
        {
            var result = await _service.GetLeaderboardAsync(competitionId, disciplineId, categoryId);
            return Ok(result);
        }

        [HttpGet("competition/{competitionId}/country-tally")]
        public async Task<IActionResult> GetCountryMedalTally(int competitionId)
        {
            var result = await _service.GetCountryMedalTallyAsync(competitionId);
            return Ok(result);
        }

        [HttpGet("discipline/{disciplineId}/records")]
        public async Task<IActionResult> GetDisciplineRecords(int disciplineId, [FromQuery] int topN = 10)
        {
            if (topN > 100) topN = 100;

            var result = await _service.GetTopRecordsByDisciplineAsync(disciplineId, topN);
            return Ok(result);
        }
    }
}
