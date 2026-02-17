using ChuksKitchen.Domain.Enums;

namespace ChuksKitchen.Application.DTOs.Responses;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public UserRole Role { get; set; }
    public string ReferralCode { get; set; } = string.Empty;
    private string? _fullName;
    public string FullName
    {
        get => _fullName ?? $"{FirstName} {LastName}";
        set
        {
            if (value != _fullName)
                _fullName = value;
        }
    }
}
