using ChuksKitchen.Application.DTOs.Requests;
using ChuksKitchen.Application.DTOs.Responses;

namespace ChuksKitchen.Application.Services.Interfaces;

public interface IOrderService
{
    Task<ResponseDto<OrderResponseDto>> CreateOrderAsync(int userId, CreateOrderRequestDto request);
    Task<ResponseDto<OrderResponseDto>> GetOrderByIdAsync(int orderId, int userId);
    Task<ResponseDto<OrderResponseDto>> GetOrderByOrderNumberAsync(string orderNumber);
    Task<ResponseDto<List<OrderResponseDto>>> GetUserOrdersAsync(int userId);
    Task<ResponseDto<List<OrderResponseDto>>> GetAllOrdersAsync();
    Task<ResponseDto<OrderResponseDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequestDto request);
    Task<ResponseDto<OrderResponseDto>> CancelOrderAsync(int orderId, int userId, string? reason);
}
