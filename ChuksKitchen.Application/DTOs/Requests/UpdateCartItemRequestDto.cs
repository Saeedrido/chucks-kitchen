namespace ChuksKitchen.Application.DTOs.Requests;

public class UpdateCartItemRequestDto
{
    public int CartItemId { get; set; }
    public int Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
}
