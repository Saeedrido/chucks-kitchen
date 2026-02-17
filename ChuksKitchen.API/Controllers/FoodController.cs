using ChuksKitchen.Application.DTOs.Requests;
using ChuksKitchen.Application.DTOs.Responses;
using ChuksKitchen.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChuksKitchen.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class FoodController : ControllerBase
{
    private readonly IFoodService _foodService;
    private readonly ILogger<FoodController> _logger;

    public FoodController(IFoodService foodService, ILogger<FoodController> logger)
    {
        _foodService = foodService;
        _logger = logger;
    }

    /// <summary>
    /// Get all food items (Admin)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ResponseDto<List<FoodItemResponseDto>>>> GetAllFoodItems()
    {
        try
        {
            var result = await _foodService.GetAllFoodItemsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all food items");
            return StatusCode(500, ResponseDto<List<FoodItemResponseDto>>.ErrorResponse("An error occurred while retrieving food items"));
        }
    }

    /// <summary>
    /// Get available food items for customers
    /// </summary>
    [HttpGet("available")]
    public async Task<ActionResult<ResponseDto<List<FoodItemResponseDto>>>> GetAvailableFoodItems()
    {
        try
        {
            var result = await _foodService.GetAvailableFoodItemsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available food items");
            return StatusCode(500, ResponseDto<List<FoodItemResponseDto>>.ErrorResponse("An error occurred while retrieving food items"));
        }
    }

    /// <summary>
    /// Get food items by category
    /// </summary>
    [HttpGet("category/{category}")]
    public async Task<ActionResult<ResponseDto<List<FoodItemResponseDto>>>> GetFoodItemsByCategory(string category)
    {
        try
        {
            var result = await _foodService.GetFoodItemsByCategoryAsync(category);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving food items for category {Category}", category);
            return StatusCode(500, ResponseDto<List<FoodItemResponseDto>>.ErrorResponse("An error occurred while retrieving food items"));
        }
    }

    /// <summary>
    /// Get food item by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseDto<FoodItemResponseDto>>> GetFoodItemById(int id)
    {
        try
        {
            var result = await _foodService.GetFoodItemByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving food item {FoodItemId}", id);
            return StatusCode(500, ResponseDto<FoodItemResponseDto>.ErrorResponse("An error occurred while retrieving the food item"));
        }
    }

    /// <summary>
    /// Create new food item (Admin)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ResponseDto<FoodItemResponseDto>>> CreateFoodItem(
        [FromBody] CreateFoodItemRequestDto request,
        [FromHeader] int adminId)
    {
        try
        {
            var result = await _foodService.CreateFoodItemAsync(request, adminId);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetFoodItemById), new { id = result.Data!.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating food item");
            return StatusCode(500, ResponseDto<FoodItemResponseDto>.ErrorResponse("An error occurred while creating the food item"));
        }
    }

    /// <summary>
    /// Update food item (Admin)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseDto<FoodItemResponseDto>>> UpdateFoodItem(
        int id,
        [FromBody] UpdateFoodItemRequestDto request)
    {
        try
        {
            var result = await _foodService.UpdateFoodItemAsync(id, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating food item {FoodItemId}", id);
            return StatusCode(500, ResponseDto<FoodItemResponseDto>.ErrorResponse("An error occurred while updating the food item"));
        }
    }

    /// <summary>
    /// Delete food item (Admin)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseDto<bool>>> DeleteFoodItem(int id)
    {
        try
        {
            var result = await _foodService.DeleteFoodItemAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting food item {FoodItemId}", id);
            return StatusCode(500, ResponseDto<bool>.ErrorResponse("An error occurred while deleting the food item"));
        }
    }
}
