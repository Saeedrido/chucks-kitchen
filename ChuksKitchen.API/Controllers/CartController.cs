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
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    /// <summary>
    /// Get user's cart
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ResponseDto<CartResponseDto>>> GetCart()
    {
        try
        {
            var userId = HttpContext.GetUserId();
            var result = await _cartService.GetCartAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cart");
            return StatusCode(500, ResponseDto<CartResponseDto>.ErrorResponse("An error occurred while retrieving the cart"));
        }
    }

    /// <summary>
    /// Add item to cart
    /// </summary>
    [HttpPost("add")]
    public async Task<ActionResult<ResponseDto<CartResponseDto>>> AddToCart([FromBody] AddToCartRequestDto request)
    {
        try
        {
            var userId = HttpContext.GetUserId();
            var result = await _cartService.AddToCartAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart");
            return StatusCode(500, ResponseDto<CartResponseDto>.ErrorResponse("An error occurred while adding the item to cart"));
        }
    }

    /// <summary>
    /// Update cart item
    /// </summary>
    [HttpPut("update")]
    public async Task<ActionResult<ResponseDto<CartResponseDto>>> UpdateCartItem([FromBody] UpdateCartItemRequestDto request)
    {
        try
        {
            var userId = HttpContext.GetUserId();
            var result = await _cartService.UpdateCartItemAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart item");
            return StatusCode(500, ResponseDto<CartResponseDto>.ErrorResponse("An error occurred while updating the cart item"));
        }
    }

    /// <summary>
    /// Remove item from cart
    /// </summary>
    [HttpDelete("remove/{cartItemId}")]
    public async Task<ActionResult<ResponseDto<CartResponseDto>>> RemoveFromCart(int cartItemId)
    {
        try
        {
            var userId = HttpContext.GetUserId();
            var result = await _cartService.RemoveFromCartAsync(userId, cartItemId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cart item {CartItemId}", cartItemId);
            return StatusCode(500, ResponseDto<CartResponseDto>.ErrorResponse("An error occurred while removing the item from cart"));
        }
    }

    /// <summary>
    /// Clear cart
    /// </summary>
    [HttpDelete("clear")]
    public async Task<ActionResult<ResponseDto<CartResponseDto>>> ClearCart()
    {
        try
        {
            var userId = HttpContext.GetUserId();
            var result = await _cartService.ClearCartAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart");
            return StatusCode(500, ResponseDto<CartResponseDto>.ErrorResponse("An error occurred while clearing the cart"));
        }
    }
}
