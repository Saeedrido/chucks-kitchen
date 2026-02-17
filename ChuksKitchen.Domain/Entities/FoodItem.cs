namespace ChuksKitchen.Domain.Entities;

public class FoodItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public int PreparationTimeMinutes { get; set; } = 15;
    public int StockQuantity { get; set; } = 100;
    public string? SpiceLevel { get; set; }

    // Foreign key
    public int AddedByAdminId { get; set; }
    public User AddedByAdmin { get; set; } = null!;

    // Navigation properties
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
