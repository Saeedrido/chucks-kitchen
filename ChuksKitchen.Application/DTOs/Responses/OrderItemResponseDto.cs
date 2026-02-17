namespace ChuksKitchen.Application.DTOs.Responses;

public class OrderItemResponseDto
{
    public int Id { get; set; }
    public int FoodItemId { get; set; }
    public string FoodItemName { get; set; } = string.Empty;
    public string? FoodItemImage { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? SpecialInstructions { get; set; }
}
