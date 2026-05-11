using CompetitionsTracking.Application.DTOs.Appeal;
using CompetitionsTracking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CompetitionsTracking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppealController : ControllerBase
    {
        private readonly IAppealService _service;

        public AppealController(IAppealService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllForUserAsync(CurrentUserId(), User.IsInRole("Admin"));
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> Create([FromBody] AppealRequestDto request)
        {
            var result = await _service.CreateAsync(request, CurrentUserId(), User.IsInRole("Admin"));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> GetPending([FromQuery] int? competitionId)
        {
            var result = await _service.GetPendingAppealsForUserAsync(competitionId, CurrentUserId(), User.IsInRole("Admin"));
            return Ok(result);
        }

        [HttpGet("{id}/dossier")]
        [Authorize(Roles = "Admin,Trainee")]
        public async Task<IActionResult> GetDossier(int id)
        {
            var result = await _service.GetAppealDossierAsync(id, CurrentUserId(), User.IsInRole("Admin"));
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] AppealRequestDto request)
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

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id, [FromBody] ApproveAppealRequestDto request)
        {
            await _service.ApproveAppealAsync(id, request);
            return NoContent();
        }

        private int CurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}
