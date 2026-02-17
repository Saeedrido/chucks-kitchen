using System.ComponentModel.DataAnnotations;

namespace ChuksKitchen.Application.DTOs.Requests;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email or phone is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Email or phone must be between 3 and 100 characters")]
    public string EmailOrPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}
