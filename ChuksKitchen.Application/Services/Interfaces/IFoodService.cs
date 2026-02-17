using ChuksKitchen.Application.DTOs.Requests;
using ChuksKitchen.Application.DTOs.Responses;

namespace ChuksKitchen.Application.Services.Interfaces;

public interface IFoodService
{
    Task<ResponseDto<FoodItemResponseDto>> CreateFoodItemAsync(CreateFoodItemRequestDto request, int adminId);
    Task<ResponseDto<FoodItemResponseDto>> UpdateFoodItemAsync(int id, UpdateFoodItemRequestDto request);
    Task<ResponseDto<bool>> DeleteFoodItemAsync(int id);
    Task<ResponseDto<FoodItemResponseDto>> GetFoodItemByIdAsync(int id);
    Task<ResponseDto<List<FoodItemResponseDto>>> GetAllFoodItemsAsync();
    Task<ResponseDto<List<FoodItemResponseDto>>> GetAvailableFoodItemsAsync();
    Task<ResponseDto<List<FoodItemResponseDto>>> GetFoodItemsByCategoryAsync(string category);
}
