using System.ComponentModel.DataAnnotations;

namespace ChuksKitchen.Application.DTOs.Requests;

/// <summary>
/// Validates that a URL starts with https://
/// </summary>
public class HttpsUrlAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            // ImageUrl is optional, so null is okay
            return ValidationResult.Success;
        }

        var url = value.ToString()!;

        if (!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return new ValidationResult(
                "Image URL must start with https:// (e.g., https://example.com/image.jpg)"
            );
        }

        return ValidationResult.Success;
    }
}
