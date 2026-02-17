using System.ComponentModel.DataAnnotations;
using ChuksKitchen.Domain.Enums;

namespace ChuksKitchen.Application.DTOs.Requests;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Referral code must be a valid format like CK-AB1234")]
    public string? ReferralCode { get; set; }

    [Required(ErrorMessage = "Registration method is required")]
    public RegistrationMethod RegistrationMethod { get; set; }

    public UserRole Role { get; set; } = UserRole.Customer;
}
