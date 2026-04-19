using CompetitionsTracking.Application.DTOs.Team;
using CompetitionsTracking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompetitionsTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _service;

        public TeamController(ITeamService service)
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
        public async Task<IActionResult> Create([FromBody] TeamRequestDto request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> Update(int id, [FromBody] TeamRequestDto request)
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

        [HttpGet("{id}/metrics")]
        public async Task<IActionResult> GetTeamDominanceMetrics(int id)
        {
            var result = await _service.GetTeamDominanceMetricsAsync(id);
            return Ok(result);
        }
        [HttpGet("{id}/roster")]
        public async Task<IActionResult> GetTeamRoster(int id)
        {
            var result = await _service.GetTeamRosterAsync(id);
            if (result == null) return NotFound(new { message = "Команду не знайдено." });
            return Ok(result);
        }

        [HttpPost("{teamId}/members/{personId}")]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> AddMemberToTeam(int teamId, int personId)
        {
            await _service.AddMemberToTeamAsync(teamId, personId);
            return Ok(new { message = "Спортсмена успішно додано до команди." });
        }

        [HttpDelete("{teamId}/members/{personId}")]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> RemoveMemberFromTeam(int teamId, int personId)
        {
            await _service.RemoveMemberFromTeamAsync(teamId, personId);
            return Ok(new { message = "Спортсмена успішно видалено з команди." });
        }
    }
}
