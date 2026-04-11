using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Application.DTOs.Auth
{
    public record LoginRequestDto
    {
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }

    public record RegisterRequestDto
    {
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public UserRole Role { get; init; } = UserRole.Guest;
    }

    public record AuthResponseDto
    {
        public string Token { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public UserRole Role { get; init; }
    }
}
