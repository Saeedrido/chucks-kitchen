using ChuksKitchen.API.Extensions;
using ChuksKitchen.Application.DTOs.Responses;
using ChuksKitchen.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuksKitchen.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get user by referral code (for referral validation)
    /// </summary>
    [HttpGet("by-referral-code/{code}")]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseDto<UserReferralDto>>> GetByReferralCode(string code)
    {
        try
        {
            var result = await _userService.GetUserByReferralCodeAsync(code);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error looking up referral code");
            return StatusCode(500, ResponseDto<UserReferralDto>.ErrorResponse("Error looking up referral code"));
        }
    }

    /// <summary>
    /// Get current user profile (requires authentication)
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ResponseDto<UserProfileDto>>> GetCurrentUser()
    {
        try
        {
            var userId = HttpContext.GetUserId();
            var result = await _userService.GetUserByIdAsync(userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, ResponseDto<UserProfileDto>.ErrorResponse("Error getting current user"));
        }
    }
}
