using ChuksKitchen.Application.DTOs.Requests;
using ChuksKitchen.Application.DTOs.Responses;
using ChuksKitchen.Application.Repositories.Interfaces;
using ChuksKitchen.Application.Services.Interfaces;
using ChuksKitchen.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ChuksKitchen.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IFoodItemRepository _foodItemRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CartService> _logger;

    public CartService(
        ICartRepository cartRepository,
        IFoodItemRepository foodItemRepository,
        IUserRepository userRepository,
        ILogger<CartService> logger)
    {
        _cartRepository = cartRepository;
        _foodItemRepository = foodItemRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ResponseDto<CartResponseDto>> GetCartAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ResponseDto<CartResponseDto>.ErrorResponse("User not found");

            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                // Return empty cart
                var emptyResponse = new CartResponseDto
                {
                    Id = 0,
                    UserId = userId,
                    Items = new List<CartItemResponseDto>()
                };
                return ResponseDto<CartResponseDto>.SuccessResponse(emptyResponse);
            }

            var response = MapToCartResponse(cart);
            return ResponseDto<CartResponseDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cart for user {UserId}", userId);
            return ResponseDto<CartResponseDto>.ErrorResponse("Failed to retrieve cart");
        }
    }

    public async Task<ResponseDto<CartResponseDto>> AddToCartAsync(int userId, AddToCartRequestDto request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ResponseDto<CartResponseDto>.ErrorResponse("User not found");

            var foodItem = await _foodItemRepository.GetByIdAsync(request.FoodItemId);
            if (foodItem == null)
                return ResponseDto<CartResponseDto>.ErrorResponse("Food item not found");

            // Business Rule: Check if food item is available
            if (!foodItem.IsAvailable)
                return ResponseDto<CartResponseDto>.ErrorResponse("This food item is currently unavailable");

            // Business Rule: Check stock availability
            if (foodItem.StockQuantity < request.Quantity)
                return ResponseDto<CartResponseDto>.ErrorResponse($"Insufficient stock. Only {foodItem.StockQuantity} items available");

            // Business Rule: Validate quantity
            if (request.Quantity <= 0)
                return ResponseDto<CartResponseDto>.ErrorResponse("Quantity must be greater than zero");

            var cart = await _cartRepository.GetOrCreateCartAsync(userId);

            // Business Rule: Check if item already exists in cart
            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.FoodItemId == request.FoodItemId);

            if (existingCartItem != null)
            {
                // Business Rule: Update quantity if item exists
                var newQuantity = existingCartItem.Quantity + request.Quantity;

                // Business Rule: Check stock again for new quantity
                if (foodItem.StockQuantity < newQuantity)
                    return ResponseDto<CartResponseDto>.ErrorResponse($"Insufficient stock. You can only add {foodItem.StockQuantity - existingCartItem.Quantity} more items");

                existingCartItem.Quantity = newQuantity;
                existingCartItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Add new cart item
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    FoodItemId = request.FoodItemId,
                    Quantity = request.Quantity,
                    UnitPrice = foodItem.Price
                };

                cart.CartItems.Add(cartItem);
            }

            await _cartRepository.Update(cart);

            // Refresh cart with items
            cart = await _cartRepository.GetWithItemsAsync(cart.Id);
            var response = MapToCartResponse(cart!);

            return ResponseDto<CartResponseDto>.SuccessResponse(response, "Item added to cart successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart for user {UserId}", userId);
            return ResponseDto<CartResponseDto>.ErrorResponse("Failed to add item to cart");
        }
    }

    public async Task<ResponseDto<CartResponseDto>> UpdateCartItemAsync(int userId, UpdateCartItemRequestDto request)
    {
        try
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
                return ResponseDto<CartResponseDto>.ErrorResponse("Cart not found");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == request.CartItemId);
            if (cartItem == null)
                return ResponseDto<CartResponseDto>.ErrorResponse("Cart item not found");

            var foodItem = await _foodItemRepository.GetByIdAsync(cartItem.FoodItemId);
            if (foodItem == null)
                return ResponseDto<CartResponseDto>.ErrorResponse("Food item not found");

            // Business Rule: Check if food became unavailable after being added to cart
            if (!foodItem.IsAvailable)
                return ResponseDto<CartResponseDto>.ErrorResponse("This food item is now unavailable. Please remove it from your cart");

            // Business Rule: Check stock availability
            if (foodItem.StockQuantity < request.Quantity)
                return ResponseDto<CartResponseDto>.ErrorResponse($"Insufficient stock. Only {foodItem.StockQuantity} items available");

            // Business Rule: Validate quantity
            if (request.Quantity <= 0)
                return ResponseDto<CartResponseDto>.ErrorResponse("Quantity must be greater than zero. To remove item, use remove endpoint");

            cartItem.Quantity = request.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.Update(cart);

            // Refresh cart
            cart = await _cartRepository.GetWithItemsAsync(cart.Id);
            var response = MapToCartResponse(cart!);

            return ResponseDto<CartResponseDto>.SuccessResponse(response, "Cart item updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart item {CartItemId}", request.CartItemId);
            return ResponseDto<CartResponseDto>.ErrorResponse("Failed to update cart item");
        }
    }

    public async Task<ResponseDto<CartResponseDto>> RemoveFromCartAsync(int userId, int cartItemId)
    {
        try
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
                return ResponseDto<CartResponseDto>.ErrorResponse("Cart not found");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
                return ResponseDto<CartResponseDto>.ErrorResponse("Cart item not found");

            cart.CartItems.Remove(cartItem);
            await _cartRepository.Update(cart);

            // Refresh cart
            cart = await _cartRepository.GetWithItemsAsync(cart.Id);
            var response = MapToCartResponse(cart!);

            return ResponseDto<CartResponseDto>.SuccessResponse(response, "Item removed from cart successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cart item {CartItemId}", cartItemId);
            return ResponseDto<CartResponseDto>.ErrorResponse("Failed to remove item from cart");
        }
    }

    public async Task<ResponseDto<CartResponseDto>> ClearCartAsync(int userId)
    {
        try
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
                return ResponseDto<CartResponseDto>.ErrorResponse("Cart not found");

            cart.CartItems.Clear();
            await _cartRepository.Update(cart);

            var response = MapToCartResponse(cart);
            return ResponseDto<CartResponseDto>.SuccessResponse(response, "Cart cleared successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
            return ResponseDto<CartResponseDto>.ErrorResponse("Failed to clear cart");
        }
    }

    private CartResponseDto MapToCartResponse(Cart cart)
    {
        return new CartResponseDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = cart.CartItems.Select(ci => new CartItemResponseDto
            {
                Id = ci.Id,
                FoodItemId = ci.FoodItemId,
                FoodItemName = ci.FoodItem?.Name ?? "Unknown",
                FoodItemImage = ci.FoodItem?.ImageUrl,
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice,
                TotalPrice = ci.TotalPrice,
                SpecialInstructions = ci.SpecialInstructions,
                IsAvailable = ci.FoodItem?.IsAvailable ?? false
            }).ToList()
        };
    }
}
