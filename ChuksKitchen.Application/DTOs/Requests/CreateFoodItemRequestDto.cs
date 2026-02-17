using System.ComponentModel.DataAnnotations;

namespace ChuksKitchen.Application.DTOs.Requests;

public class CreateFoodItemRequestDto
{
    [Required(ErrorMessage = "Food name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Food name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 100000, ErrorMessage = "Price must be between 0.01 and 100,000")]
    public decimal Price { get; set; }

    [HttpsUrl(ErrorMessage = "Image URL must start with https:// (e.g., https://example.com/image.jpg)")]
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Category must be between 2 and 50 characters")]
    public string Category { get; set; } = string.Empty;

    [Range(1, 180, ErrorMessage = "Preparation time must be between 1 and 180 minutes")]
    public int PreparationTimeMinutes { get; set; } = 15;

    [Range(0, 10000, ErrorMessage = "Stock quantity must be between 0 and 10,000")]
    public int StockQuantity { get; set; } = 100;

    [StringLength(20, ErrorMessage = "Spice level must not exceed 20 characters")]
    public string? SpiceLevel { get; set; }
}
