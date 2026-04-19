using CompetitionsTracking.Application.DTOs.Person;
using CompetitionsTracking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompetitionsTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _service;

        public PersonController(IPersonService service)
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
        public async Task<IActionResult> Create([FromBody] PersonRequestDto request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> Update(int id, [FromBody] PersonRequestDto request)
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

        [HttpGet("{id}/performance")]
        public async Task<IActionResult> GetParticipantPerformanceHistory(int id)
        {
            var result = await _service.GetParticipantPerformanceHistoryAsync(id);
            return Ok(result);
        }
        [HttpGet("{id}/mentees")]
        public async Task<IActionResult> GetMentees(int id)
        {
            var result = await _service.GetMenteesAsync(id);
            if (!result.Any())
            {
                return Ok(new { message = "У цього учасника немає підопічних." });
            }
            return Ok(result);
        }

        [HttpGet("{id}/teams")]
        public async Task<IActionResult> GetTeamAffiliations(int id)
        {
            var result = await _service.GetTeamAffiliationsAsync(id);
            if (!result.Any())
            {
                return Ok(new { message = "Цей учасник не входить до жодної команди і не тренує їх." });
            }
            return Ok(result);
        }
    }
}
