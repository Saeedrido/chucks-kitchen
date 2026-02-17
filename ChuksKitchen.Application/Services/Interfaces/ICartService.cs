using ChuksKitchen.Application.DTOs.Requests;
using ChuksKitchen.Application.DTOs.Responses;

namespace ChuksKitchen.Application.Services.Interfaces;

public interface ICartService
{
    Task<ResponseDto<CartResponseDto>> GetCartAsync(int userId);
    Task<ResponseDto<CartResponseDto>> AddToCartAsync(int userId, AddToCartRequestDto request);
    Task<ResponseDto<CartResponseDto>> UpdateCartItemAsync(int userId, UpdateCartItemRequestDto request);
    Task<ResponseDto<CartResponseDto>> RemoveFromCartAsync(int userId, int cartItemId);
    Task<ResponseDto<CartResponseDto>> ClearCartAsync(int userId);
}
