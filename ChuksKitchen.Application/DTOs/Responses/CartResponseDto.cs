namespace ChuksKitchen.Application.DTOs.Responses;

public class CartResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<CartItemResponseDto> Items { get; set; } = new();
    public int TotalItems => Items.Sum(i => i.Quantity);
    public decimal SubTotal => Items.Sum(i => i.TotalPrice);
    public decimal DeliveryFee => 500m;
    public decimal TotalAmount => SubTotal + DeliveryFee;
}
