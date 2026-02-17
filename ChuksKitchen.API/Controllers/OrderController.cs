using ChuksKitchen.API.Extensions;
using ChuksKitchen.Application.DTOs.Requests;
using ChuksKitchen.Application.DTOs.Responses;
using ChuksKitchen.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuksKitchen.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ResponseDto<OrderResponseDto>>> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        try
        {
            var userId = HttpContext.GetUserId();
            var result = await _orderService.CreateOrderAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetOrderById), new { orderId = result.Data!.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, ResponseDto<OrderResponseDto>.ErrorResponse("An error occurred while creating the order"));
        }
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{orderId}")]
    public async Task<ActionResult<ResponseDto<OrderResponseDto>>> GetOrderById(int orderId)
    {
        try
        {
            var userId = HttpContext.GetUserId();
            var result = await _orderService.GetOrderByIdAsync(orderId, userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {OrderId}", orderId);
            return StatusCode(500, ResponseDto<OrderResponseDto>.ErrorResponse("An error occurred while retrieving the order"));
        }
    }

    /// <summary>
    /// Get order by order number
    /// </summary>
    [HttpGet("number/{orderNumber}")]
    public async Task<ActionResult<ResponseDto<OrderResponseDto>>> GetOrderByOrderNumber(string orderNumber)
    {
        try
        {
            var result = await _orderService.GetOrderByOrderNumberAsync(orderNumber);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {OrderNumber}", orderNumber);
            return StatusCode(500, ResponseDto<OrderResponseDto>.ErrorResponse("An error occurred while retrieving the order"));
        }
    }

    /// <summary>
    /// Get user's orders
    /// </summary>
    [HttpGet("user")]
    public async Task<ActionResult<ResponseDto<List<OrderResponseDto>>>> GetUserOrders()
    {
        try
        {
            var userId = HttpContext.GetUserId();
            var result = await _orderService.GetUserOrdersAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return StatusCode(500, ResponseDto<List<OrderResponseDto>>.ErrorResponse("An error occurred while retrieving orders"));
        }
    }

    /// <summary>
    /// Get all orders (Admin)
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<ResponseDto<List<OrderResponseDto>>>> GetAllOrders()
    {
        try
        {
            var result = await _orderService.GetAllOrdersAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all orders");
            return StatusCode(500, ResponseDto<List<OrderResponseDto>>.ErrorResponse("An error occurred while retrieving orders"));
        }
    }

    /// <summary>
    /// Update order status (Admin)
    /// </summary>
    [HttpPut("{orderId}/status")]
    public async Task<ActionResult<ResponseDto<OrderResponseDto>>> UpdateOrderStatus(
        int orderId,
        [FromBody] UpdateOrderStatusRequestDto request)
    {
        try
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for order {OrderId}", orderId);
            return StatusCode(500, ResponseDto<OrderResponseDto>.ErrorResponse("An error occurred while updating the order status"));
        }
    }

    /// <summary>
    /// Cancel order
    /// </summary>
    [HttpPost("{orderId}/cancel")]
    public async Task<ActionResult<ResponseDto<OrderResponseDto>>> CancelOrder(
        int orderId,
        [FromBody] Dictionary<string, string>? request)
    {
        try
        {
            var userId = HttpContext.GetUserId();
            var reason = request?.GetValueOrDefault("reason");
            var result = await _orderService.CancelOrderAsync(orderId, userId, reason);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {OrderId}", orderId);
            return StatusCode(500, ResponseDto<OrderResponseDto>.ErrorResponse("An error occurred while cancelling the order"));
        }
    }
}
