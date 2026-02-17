namespace ChuksKitchen.Domain.Entities;

public class CartItem : BaseEntity
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
    public string? SpecialInstructions { get; set; }

    // Foreign keys
    public int CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public int FoodItemId { get; set; }
    public FoodItem FoodItem { get; set; } = null!;
}
