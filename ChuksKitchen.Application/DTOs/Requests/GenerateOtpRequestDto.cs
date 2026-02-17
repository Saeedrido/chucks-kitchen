using System.ComponentModel.DataAnnotations;

namespace ChuksKitchen.Application.DTOs.Requests;

public class GenerateOtpRequestDto
{
    [Required]
    public string EmailOrPhone { get; set; } = string.Empty;
}
