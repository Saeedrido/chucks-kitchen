using ChuksKitchen.Domain.Enums;

namespace ChuksKitchen.Application.DTOs.Responses;

public class OrderResponseDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public string StatusDisplay => Status.ToString();
    public decimal TotalAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? SpecialInstructions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? PreparingAt { get; set; }
    public DateTime? OutForDeliveryAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public bool IsPaid { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
    public UserSummaryResponseDto? Customer { get; set; }
}

public class UserSummaryResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
