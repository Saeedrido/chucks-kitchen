namespace ChuksKitchen.Application.DTOs.Requests;

public class UpdateFoodItemRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public int PreparationTimeMinutes { get; set; }
    public int StockQuantity { get; set; }
    public string? SpiceLevel { get; set; }
}
