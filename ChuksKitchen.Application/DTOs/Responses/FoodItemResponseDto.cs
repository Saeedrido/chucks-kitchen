namespace ChuksKitchen.Application.DTOs.Responses;

public class FoodItemResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public int PreparationTimeMinutes { get; set; }
    public int StockQuantity { get; set; }
    public string? SpiceLevel { get; set; }
    public string? AddedByAdmin { get; set; }
}
