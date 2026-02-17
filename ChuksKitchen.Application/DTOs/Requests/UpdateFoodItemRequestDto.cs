using System.ComponentModel.DataAnnotations;

namespace ChuksKitchen.Application.DTOs.Requests;

public class UpdateFoodItemRequestDto
{
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Food name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 100000, ErrorMessage = "Price must be between 0.01 and 100,000")]
    public decimal Price { get; set; }

    [HttpsUrl(ErrorMessage = "Image URL must start with https:// (e.g., https://example.com/image.jpg)")]
    public string? ImageUrl { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "Category must be between 2 and 50 characters")]
    public string Category { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    [Range(1, 180, ErrorMessage = "Preparation time must be between 1 and 180 minutes")]
    public int PreparationTimeMinutes { get; set; }

    [Range(0, 10000, ErrorMessage = "Stock quantity must be between 0 and 10,000")]
    public int StockQuantity { get; set; }

    [StringLength(20, ErrorMessage = "Spice level must not exceed 20 characters")]
    public string? SpiceLevel { get; set; }
}
