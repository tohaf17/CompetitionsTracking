using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Application.DTOs.Auth
{
    public record LoginRequestDto
    {
        public string Identifier { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }

    public record RegisterRequestDto
    {
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public UserRole Role { get; init; } = UserRole.Guest;
    }

    public record AuthResponseDto
    {
        public string Token { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public UserRole Role { get; init; }
    }

    public record UserDto
    {
        public int Id { get; init; }
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
