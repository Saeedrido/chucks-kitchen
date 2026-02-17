using System.ComponentModel.DataAnnotations;

namespace ChuksKitchen.Application.DTOs.Requests;

public class CreateOrderRequestDto
{
    [Required(ErrorMessage = "Delivery address is required")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Delivery address must be between 10 and 500 characters")]
    public string DeliveryAddress { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Special instructions must not exceed 500 characters")]
    public string? SpecialInstructions { get; set; }
}
