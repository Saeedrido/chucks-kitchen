using ChuksKitchen.Application.DTOs.Requests;
using ChuksKitchen.Application.DTOs.Responses;
using ChuksKitchen.Application.Repositories.Interfaces;
using ChuksKitchen.Application.Services.Interfaces;
using ChuksKitchen.Domain.Entities;
using ChuksKitchen.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ChuksKitchen.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IFoodItemRepository _foodItemRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IFoodItemRepository foodItemRepository,
        IUserRepository userRepository,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _foodItemRepository = foodItemRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ResponseDto<OrderResponseDto>> CreateOrderAsync(int userId, CreateOrderRequestDto request)
    {
        try
        {
            // Business Rule: Validate user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ResponseDto<OrderResponseDto>.ErrorResponse("User not found");

            // Business Rule: Get user's cart
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                return ResponseDto<OrderResponseDto>.ErrorResponse("Your cart is empty");

            // Business Rule: Validate all items are available and in stock
            foreach (var cartItem in cart.CartItems)
            {
                if (!cartItem.FoodItem.IsAvailable)
                    return ResponseDto<OrderResponseDto>.ErrorResponse($"{cartItem.FoodItem.Name} is now unavailable. Please remove it from your cart");

                if (cartItem.FoodItem.StockQuantity < cartItem.Quantity)
                    return ResponseDto<OrderResponseDto>.ErrorResponse($"Insufficient stock for {cartItem.FoodItem.Name}. Available: {cartItem.FoodItem.StockQuantity}");
            }

            // Business Rule: Calculate total amount
            var subTotal = cart.CartItems.Sum(ci => ci.TotalPrice);
            var deliveryFee = 500m;
            var totalAmount = subTotal + deliveryFee;

            // Business Rule: Generate unique order number
            var orderNumber = await GenerateOrderNumberAsync();

            // Create order
            var order = new Order
            {
                OrderNumber = orderNumber,
                UserId = userId,
                Status = OrderStatus.Pending,
                TotalAmount = totalAmount,
                DeliveryFee = deliveryFee,
                DeliveryAddress = request.DeliveryAddress,
                SpecialInstructions = request.SpecialInstructions,
                IsPaid = false
            };

            // Business Rule: Create order items from cart
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    FoodItemId = cartItem.FoodItemId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    SpecialInstructions = cartItem.SpecialInstructions
                };

                order.OrderItems.Add(orderItem);

                // Business Rule: Update stock quantity
                cartItem.FoodItem.StockQuantity -= cartItem.Quantity;
                await _foodItemRepository.Update(cartItem.FoodItem);
            }

            var createdOrder = await _orderRepository.AddAsync(order);

            // Business Rule: Clear cart after order is placed
            cart.CartItems.Clear();
            await _cartRepository.Update(cart);

            // Refresh order with items
            createdOrder = await _orderRepository.GetWithItemsAsync(createdOrder.Id);
            var response = MapToOrderResponse(createdOrder!);

            return ResponseDto<OrderResponseDto>.SuccessResponse(response, "Order placed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for user {UserId}", userId);
            return ResponseDto<OrderResponseDto>.ErrorResponse("Failed to create order");
        }
    }

    public async Task<ResponseDto<OrderResponseDto>> GetOrderByIdAsync(int orderId, int userId)
    {
        try
        {
            var order = await _orderRepository.GetUserOrderAsync(userId, orderId);
            if (order == null)
                return ResponseDto<OrderResponseDto>.ErrorResponse("Order not found");

            var response = MapToOrderResponse(order);
            return ResponseDto<OrderResponseDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {OrderId}", orderId);
            return ResponseDto<OrderResponseDto>.ErrorResponse("Failed to retrieve order");
        }
    }

    public async Task<ResponseDto<OrderResponseDto>> GetOrderByOrderNumberAsync(string orderNumber)
    {
        try
        {
            var order = await _orderRepository.GetByOrderNumberAsync(orderNumber);
            if (order == null)
                return ResponseDto<OrderResponseDto>.ErrorResponse("Order not found");

            var response = MapToOrderResponse(order);
            return ResponseDto<OrderResponseDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {OrderNumber}", orderNumber);
            return ResponseDto<OrderResponseDto>.ErrorResponse("Failed to retrieve order");
        }
    }

    public async Task<ResponseDto<List<OrderResponseDto>>> GetUserOrdersAsync(int userId)
    {
        try
        {
            var orders = await _orderRepository.GetUserOrdersAsync(userId);
            var response = orders.Select(MapToOrderResponse).ToList();

            return ResponseDto<List<OrderResponseDto>>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders for user {UserId}", userId);
            return ResponseDto<List<OrderResponseDto>>.ErrorResponse("Failed to retrieve orders");
        }
    }

    public async Task<ResponseDto<List<OrderResponseDto>>> GetAllOrdersAsync()
    {
        try
        {
            var orders = await _orderRepository.GetAllAsync();
            var response = new List<OrderResponseDto>();

            foreach (var order in orders)
            {
                var orderWithItems = await _orderRepository.GetWithItemsAsync(order.Id);
                if (orderWithItems != null)
                    response.Add(MapToOrderResponse(orderWithItems));
            }

            return ResponseDto<List<OrderResponseDto>>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all orders");
            return ResponseDto<List<OrderResponseDto>>.ErrorResponse("Failed to retrieve orders");
        }
    }

    public async Task<ResponseDto<OrderResponseDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequestDto request)
    {
        try
        {
            var order = await _orderRepository.GetWithItemsAsync(orderId);
            if (order == null)
                return ResponseDto<OrderResponseDto>.ErrorResponse("Order not found");

            // Business Rule: Validate status transitions
            if (!IsValidStatusTransition(order.Status, request.NewStatus))
                return ResponseDto<OrderResponseDto>.ErrorResponse($"Invalid status transition from {order.Status} to {request.NewStatus}");

            // Business Rule: Update timestamp based on status
            order.Status = request.NewStatus;
            order.UpdatedAt = DateTime.UtcNow;

            switch (request.NewStatus)
            {
                case OrderStatus.Confirmed:
                    order.ConfirmedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Preparing:
                    order.PreparingAt = DateTime.UtcNow;
                    break;
                case OrderStatus.OutForDelivery:
                    order.OutForDeliveryAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Completed:
                    order.CompletedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Cancelled:
                    order.CancelledAt = DateTime.UtcNow;
                    order.CancellationReason = request.CancellationReason;
                    // Business Rule: Restore stock when order is cancelled
                    await RestoreStockAsync(order);
                    break;
            }

            await _orderRepository.Update(order);

            var response = MapToOrderResponse(order);
            return ResponseDto<OrderResponseDto>.SuccessResponse(response, $"Order status updated to {request.NewStatus}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status for order {OrderId}", orderId);
            return ResponseDto<OrderResponseDto>.ErrorResponse("Failed to update order status");
        }
    }

    public async Task<ResponseDto<OrderResponseDto>> CancelOrderAsync(int orderId, int userId, string? reason)
    {
        try
        {
            var order = await _orderRepository.GetUserOrderAsync(userId, orderId);
            if (order == null)
                return ResponseDto<OrderResponseDto>.ErrorResponse("Order not found");

            // Business Rule: Can only cancel pending or confirmed orders
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                return ResponseDto<OrderResponseDto>.ErrorResponse($"Cannot cancel order with status {order.Status}");

            // Business Rule: Restore stock
            await RestoreStockAsync(order);

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;
            order.CancellationReason = reason ?? "Cancelled by customer";
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.Update(order);

            var response = MapToOrderResponse(order);
            return ResponseDto<OrderResponseDto>.SuccessResponse(response, "Order cancelled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {OrderId}", orderId);
            return ResponseDto<OrderResponseDto>.ErrorResponse("Failed to cancel order");
        }
    }

    private async Task<string> GenerateOrderNumberAsync()
    {
        // Business Rule: Generate unique order number (CK + timestamp + random)
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"CK{timestamp}{random}";
    }

    private bool IsValidStatusTransition(OrderStatus current, OrderStatus next)
    {
        // Business Rule: Define valid status transitions
        return (current, next) switch
        {
            (OrderStatus.Pending, OrderStatus.Confirmed) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Confirmed, OrderStatus.Preparing) => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
            (OrderStatus.Preparing, OrderStatus.OutForDelivery) => true,
            (OrderStatus.OutForDelivery, OrderStatus.Completed) => true,
            _ => false
        };
    }

    private async Task RestoreStockAsync(Order order)
    {
        foreach (var orderItem in order.OrderItems)
        {
            var foodItem = await _foodItemRepository.GetByIdAsync(orderItem.FoodItemId);
            if (foodItem != null)
            {
                foodItem.StockQuantity += orderItem.Quantity;
                await _foodItemRepository.Update(foodItem);
            }
        }
    }

    private OrderResponseDto MapToOrderResponse(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            DeliveryFee = order.DeliveryFee,
            DeliveryAddress = order.DeliveryAddress,
            SpecialInstructions = order.SpecialInstructions,
            CreatedAt = order.CreatedAt,
            ConfirmedAt = order.ConfirmedAt,
            PreparingAt = order.PreparingAt,
            OutForDeliveryAt = order.OutForDeliveryAt,
            CompletedAt = order.CompletedAt,
            CancelledAt = order.CancelledAt,
            CancellationReason = order.CancellationReason,
            IsPaid = order.IsPaid,
            Items = order.OrderItems.Select(oi => new OrderItemResponseDto
            {
                Id = oi.Id,
                FoodItemId = oi.FoodItemId,
                FoodItemName = oi.FoodItem?.Name ?? "Unknown",
                FoodItemImage = oi.FoodItem?.ImageUrl,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice,
                SpecialInstructions = oi.SpecialInstructions
            }).ToList(),
            Customer = new UserSummaryResponseDto
            {
                Id = order.User.Id,
                FullName = $"{order.User.FirstName} {order.User.LastName}",
                Email = order.User.Email,
                Phone = order.User.Phone
            }
        };
    }
}
