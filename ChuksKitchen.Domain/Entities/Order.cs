using ChuksKitchen.Domain.Enums;

namespace ChuksKitchen.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public decimal DeliveryFee { get; set; } = 500m;
    public string? DeliveryAddress { get; set; }
    public string? SpecialInstructions { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? PreparingAt { get; set; }
    public DateTime? OutForDeliveryAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public bool IsPaid { get; set; } = false;

    // Foreign key
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // Navigation properties
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
