using CompetitionsTracking.Application.DTOs.Person;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IPersonService
    {
        Task<IEnumerable<PersonResponseDto>> GetAllAsync();
        Task<PersonResponseDto?> GetByIdAsync(int id);
        Task<PersonResponseDto> CreateAsync(PersonRequestDto request);
        Task UpdateAsync(int id, PersonRequestDto request);
        Task DeleteAsync(int id);
    }
}
