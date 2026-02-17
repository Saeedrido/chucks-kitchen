using System.ComponentModel.DataAnnotations;

namespace ChuksKitchen.Application.DTOs.Requests;

public class AddToCartRequestDto
{
    [Required(ErrorMessage = "Food item ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Food item ID must be positive")]
    public int FoodItemId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    public int Quantity { get; set; } = 1;

    [StringLength(500, ErrorMessage = "Special instructions must not exceed 500 characters")]
    public string? SpecialInstructions { get; set; }
}
