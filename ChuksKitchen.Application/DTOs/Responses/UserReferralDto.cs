using ChuksKitchen.Domain.Enums;

namespace ChuksKitchen.Application.DTOs.Responses;

public class UserReferralDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ReferralCode { get; set; } = string.Empty;
}

public class UserProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public UserRole Role { get; set; }
    public string RoleDisplay { get; set; } = string.Empty;
    public string ReferralCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int ReferralCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
