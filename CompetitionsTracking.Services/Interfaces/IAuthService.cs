using CompetitionsTracking.Application.DTOs.Auth;
using System.Threading.Tasks;

namespace CompetitionsTracking.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    }
}
