$entities = @("Apparatus", "Appeal", "Category", "Competition", "Discipline", "Entry", "Judge", "Person", "Result", "Score", "Team")
$basePath = "c:\Users\filip\OneDrive\Desktop\2 term\CourseWork\CompetitionsTracking"

foreach ($entity in $entities) {
    # DTOs
    $dtoDir = "$basePath\CompetitionsTracking.Application\DTOs\$entity"
    New-Item -ItemType Directory -Force -Path $dtoDir | Out-Null
    $dtoContent = @"
namespace CompetitionsTracking.Application.DTOs.$entity
{
    public class ${entity}RequestDto
    {
        // Add properties here
    }

    public class ${entity}ResponseDto
    {
        public int Id { get; set; }
        // Add properties here
    }
}
"@
    Set-Content -Path "$dtoDir\${entity}Dtos.cs" -Value $dtoContent

    # Validations
    $valDir = "$basePath\CompetitionsTracking.Application\Validators\$entity"
    New-Item -ItemType Directory -Force -Path $valDir | Out-Null
    $valContent = @"
using FluentValidation;
using CompetitionsTracking.Application.DTOs.$entity;

namespace CompetitionsTracking.Application.Validators.$entity
{
    public class ${entity}RequestDtoValidator : AbstractValidator<${entity}RequestDto>
    {
        public ${entity}RequestDtoValidator()
        {
            // Add validation rules here
        }
    }
}
"@
    Set-Content -Path "$valDir\${entity}RequestDtoValidator.cs" -Value $valContent

    # Repositories Interface
    $repoIntDir = "$basePath\CompetitionsTracking.Repositories\Interfaces"
    New-Item -ItemType Directory -Force -Path $repoIntDir | Out-Null
    $repoIntContent = @"
using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Repositories.Interfaces
{
    public interface I${entity}Repository : IRepository<$entity>
    {
    }
}
"@
    Set-Content -Path "$repoIntDir\I${entity}Repository.cs" -Value $repoIntContent

    # Repositories Implementation
    $repoImplDir = "$basePath\CompetitionsTracking.Repositories\Implementations"
    New-Item -ItemType Directory -Force -Path $repoImplDir | Out-Null
    $repoImplContent = @"
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Infrastructure.Data;
using CompetitionsTracking.Repositories.Interfaces;

namespace CompetitionsTracking.Repositories.Implementations
{
    public class ${entity}Repository : Repository<$entity>, I${entity}Repository
    {
        public ${entity}Repository(CompetitionsTrackingDbContext context) : base(context)
        {
        }
    }
}
"@
    Set-Content -Path "$repoImplDir\${entity}Repository.cs" -Value $repoImplContent

    # Service Interface
    $srvIntDir = "$basePath\CompetitionsTracking.Services\Interfaces"
    New-Item -ItemType Directory -Force -Path $srvIntDir | Out-Null
    $srvIntContent = @"
using CompetitionsTracking.Application.DTOs.$entity;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface I${entity}Service
    {
        Task<IEnumerable<${entity}ResponseDto>> GetAllAsync();
        Task<${entity}ResponseDto?> GetByIdAsync(int id);
        Task<${entity}ResponseDto> CreateAsync(${entity}RequestDto request);
        Task UpdateAsync(int id, ${entity}RequestDto request);
        Task DeleteAsync(int id);
    }
}
"@
    Set-Content -Path "$srvIntDir\I${entity}Service.cs" -Value $srvIntContent

    # Service Implementation
    $srvImplDir = "$basePath\CompetitionsTracking.Services\Implementations"
    New-Item -ItemType Directory -Force -Path $srvImplDir | Out-Null
    $srvImplContent = @"
using CompetitionsTracking.Application.DTOs.$entity;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Services.Interfaces;
using Mapster;

namespace CompetitionsTracking.Services.Implementations
{
    public class ${entity}Service : I${entity}Service
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly I${entity}Repository _repository;

        public ${entity}Service(IUnitOfWork unitOfWork, I${entity}Repository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<${entity}ResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Adapt<IEnumerable<${entity}ResponseDto>>();
        }

        public async Task<${entity}ResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.Adapt<${entity}ResponseDto>();
        }

        public async Task<${entity}ResponseDto> CreateAsync(${entity}RequestDto request)
        {
            var entity = request.Adapt<$entity>();
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Adapt<${entity}ResponseDto>();
        }

        public async Task UpdateAsync(int id, ${entity}RequestDto request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                request.Adapt(entity);
                _repository.Update(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                _repository.Remove(entity);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
"@
    Set-Content -Path "$srvImplDir\${entity}Service.cs" -Value $srvImplContent

    # Controller
    $ctrlDir = "$basePath\CompetitionsTracking\Controllers"
    New-Item -ItemType Directory -Force -Path $ctrlDir | Out-Null
    $ctrlContent = @"
using CompetitionsTracking.Application.DTOs.$entity;
using CompetitionsTracking.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CompetitionsTracking.Controllers
{
    [ApiController]
    [Route(`"api/[controller]`")]
    public class ${entity}Controller : ControllerBase
    {
        private readonly I${entity}Service _service;

        public ${entity}Controller(I${entity}Service service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet(`"{id}`")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ${entity}RequestDto request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut(`"{id}`")]
        public async Task<IActionResult> Update(int id, [FromBody] ${entity}RequestDto request)
        {
            await _service.UpdateAsync(id, request);
            return NoContent();
        }

        [HttpDelete(`"{id}`")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
"@
    Set-Content -Path "$ctrlDir\${entity}Controller.cs" -Value $ctrlContent
}
