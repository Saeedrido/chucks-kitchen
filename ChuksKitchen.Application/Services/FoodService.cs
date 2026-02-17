using ChuksKitchen.Application.DTOs.Requests;
using ChuksKitchen.Application.DTOs.Responses;
using ChuksKitchen.Application.Repositories.Interfaces;
using ChuksKitchen.Application.Services.Interfaces;
using ChuksKitchen.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ChuksKitchen.Application.Services;

public class FoodService : IFoodService
{
    private readonly IFoodItemRepository _foodItemRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<FoodService> _logger;

    public FoodService(
        IFoodItemRepository foodItemRepository,
        IUserRepository userRepository,
        ILogger<FoodService> logger)
    {
        _foodItemRepository = foodItemRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ResponseDto<FoodItemResponseDto>> CreateFoodItemAsync(CreateFoodItemRequestDto request, int adminId)
    {
        try
        {
            // Business Rule: Validate user is admin
            var admin = await _userRepository.GetByIdAsync(adminId);
            if (admin == null || admin.Role != Domain.Enums.UserRole.Admin)
                return ResponseDto<FoodItemResponseDto>.ErrorResponse("Unauthorized. Only admins can create food items");

            // Business Rule: Validate price
            if (request.Price <= 0)
                return ResponseDto<FoodItemResponseDto>.ErrorResponse("Price must be greater than zero");

            // Business Rule: Validate stock
            if (request.StockQuantity < 0)
                return ResponseDto<FoodItemResponseDto>.ErrorResponse("Stock quantity cannot be negative");

            var foodItem = new FoodItem
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                Category = request.Category,
                PreparationTimeMinutes = request.PreparationTimeMinutes,
                StockQuantity = request.StockQuantity,
                SpiceLevel = request.SpiceLevel,
                IsAvailable = true,
                AddedByAdminId = adminId
            };

            var created = await _foodItemRepository.AddAsync(foodItem);
            var response = MapToFoodItemResponse(created, admin.FirstName + " " + admin.LastName);

            return ResponseDto<FoodItemResponseDto>.SuccessResponse(response, "Food item created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating food item");
            return ResponseDto<FoodItemResponseDto>.ErrorResponse("Failed to create food item");
        }
    }

    public async Task<ResponseDto<FoodItemResponseDto>> UpdateFoodItemAsync(int id, UpdateFoodItemRequestDto request)
    {
        try
        {
            var foodItem = await _foodItemRepository.GetByIdAsync(id);
            if (foodItem == null)
                return ResponseDto<FoodItemResponseDto>.ErrorResponse("Food item not found");

            // Business Rule: Validate price
            if (request.Price <= 0)
                return ResponseDto<FoodItemResponseDto>.ErrorResponse("Price must be greater than zero");

            // Business Rule: Validate stock
            if (request.StockQuantity < 0)
                return ResponseDto<FoodItemResponseDto>.ErrorResponse("Stock quantity cannot be negative");

            foodItem.Name = request.Name;
            foodItem.Description = request.Description;
            foodItem.Price = request.Price;
            foodItem.ImageUrl = request.ImageUrl;
            foodItem.Category = request.Category;
            foodItem.IsAvailable = request.IsAvailable;
            foodItem.PreparationTimeMinutes = request.PreparationTimeMinutes;
            foodItem.StockQuantity = request.StockQuantity;
            foodItem.SpiceLevel = request.SpiceLevel;
            foodItem.UpdatedAt = DateTime.UtcNow;

            await _foodItemRepository.Update(foodItem);

            var admin = await _userRepository.GetByIdAsync(foodItem.AddedByAdminId);
            var response = MapToFoodItemResponse(foodItem, admin?.FirstName + " " + admin?.LastName ?? "Unknown");

            return ResponseDto<FoodItemResponseDto>.SuccessResponse(response, "Food item updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating food item {FoodItemId}", id);
            return ResponseDto<FoodItemResponseDto>.ErrorResponse("Failed to update food item");
        }
    }

    public async Task<ResponseDto<bool>> DeleteFoodItemAsync(int id)
    {
        try
        {
            var foodItem = await _foodItemRepository.GetByIdAsync(id);
            if (foodItem == null)
                return ResponseDto<bool>.ErrorResponse("Food item not found");

            // Business Rule: Soft delete instead of hard delete
            foodItem.IsDeleted = true;
            foodItem.UpdatedAt = DateTime.UtcNow;

            await _foodItemRepository.Update(foodItem);

            return ResponseDto<bool>.SuccessResponse(true, "Food item deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting food item {FoodItemId}", id);
            return ResponseDto<bool>.ErrorResponse("Failed to delete food item");
        }
    }

    public async Task<ResponseDto<FoodItemResponseDto>> GetFoodItemByIdAsync(int id)
    {
        try
        {
            var foodItem = await _foodItemRepository.GetByIdAsync(id);
            if (foodItem == null)
                return ResponseDto<FoodItemResponseDto>.ErrorResponse("Food item not found");

            var admin = await _userRepository.GetByIdAsync(foodItem.AddedByAdminId);
            var response = MapToFoodItemResponse(foodItem, admin?.FirstName + " " + admin?.LastName ?? "Unknown");

            return ResponseDto<FoodItemResponseDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving food item {FoodItemId}", id);
            return ResponseDto<FoodItemResponseDto>.ErrorResponse("Failed to retrieve food item");
        }
    }

    public async Task<ResponseDto<List<FoodItemResponseDto>>> GetAllFoodItemsAsync()
    {
        try
        {
            var foodItems = await _foodItemRepository.GetAllAsync();
            var response = new List<FoodItemResponseDto>();

            foreach (var item in foodItems)
            {
                var admin = await _userRepository.GetByIdAsync(item.AddedByAdminId);
                response.Add(MapToFoodItemResponse(item, admin?.FirstName + " " + admin?.LastName ?? "Unknown"));
            }

            return ResponseDto<List<FoodItemResponseDto>>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all food items");
            return ResponseDto<List<FoodItemResponseDto>>.ErrorResponse("Failed to retrieve food items");
        }
    }

    public async Task<ResponseDto<List<FoodItemResponseDto>>> GetAvailableFoodItemsAsync()
    {
        try
        {
            var foodItems = await _foodItemRepository.GetAvailableItemsAsync();
            var response = new List<FoodItemResponseDto>();

            foreach (var item in foodItems)
            {
                var admin = await _userRepository.GetByIdAsync(item.AddedByAdminId);
                response.Add(MapToFoodItemResponse(item, admin?.FirstName + " " + admin?.LastName ?? "Unknown"));
            }

            return ResponseDto<List<FoodItemResponseDto>>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available food items");
            return ResponseDto<List<FoodItemResponseDto>>.ErrorResponse("Failed to retrieve food items");
        }
    }

    public async Task<ResponseDto<List<FoodItemResponseDto>>> GetFoodItemsByCategoryAsync(string category)
    {
        try
        {
            var foodItems = await _foodItemRepository.GetByCategoryAsync(category);
            var response = new List<FoodItemResponseDto>();

            foreach (var item in foodItems)
            {
                var admin = await _userRepository.GetByIdAsync(item.AddedByAdminId);
                response.Add(MapToFoodItemResponse(item, admin?.FirstName + " " + admin?.LastName ?? "Unknown"));
            }

            return ResponseDto<List<FoodItemResponseDto>>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving food items for category {Category}", category);
            return ResponseDto<List<FoodItemResponseDto>>.ErrorResponse("Failed to retrieve food items");
        }
    }

    private FoodItemResponseDto MapToFoodItemResponse(FoodItem item, string addedByAdmin)
    {
        return new FoodItemResponseDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            ImageUrl = item.ImageUrl,
            Category = item.Category,
            IsAvailable = item.IsAvailable,
            PreparationTimeMinutes = item.PreparationTimeMinutes,
            StockQuantity = item.StockQuantity,
            SpiceLevel = item.SpiceLevel,
            AddedByAdmin = addedByAdmin
        };
    }
}
