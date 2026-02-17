namespace ChuksKitchen.Domain.Entities;

public class Cart : BaseEntity
{
    // Foreign key
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // Navigation properties
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    // Computed property (not in database)
    public decimal TotalAmount => CartItems.Sum(ci => ci.TotalPrice);
}
