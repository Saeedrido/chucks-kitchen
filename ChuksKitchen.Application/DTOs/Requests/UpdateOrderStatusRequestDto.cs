using ChuksKitchen.Domain.Enums;

namespace ChuksKitchen.Application.DTOs.Requests;

public class UpdateOrderStatusRequestDto
{
    public OrderStatus NewStatus { get; set; }
    public string? CancellationReason { get; set; }
}
