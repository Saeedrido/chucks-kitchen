namespace ChuksKitchen.Application.DTOs.Responses;

public class LoginResponseDto
{
    public UserResponseDto User { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}
