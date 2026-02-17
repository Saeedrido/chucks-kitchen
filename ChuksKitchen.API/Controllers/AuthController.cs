using ChuksKitchen.Application.DTOs.Requests;
using ChuksKitchen.Application.DTOs.Responses;
using ChuksKitchen.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChuksKitchen.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<ResponseDto<UserResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, ResponseDto<UserResponseDto>.ErrorResponse("An error occurred during registration"));
        }
    }

    /// <summary>
    /// Verify user account with OTP
    /// </summary>
    [HttpPost("verify")]
    public async Task<ActionResult<ResponseDto<UserResponseDto>>> VerifyOtp([FromBody] VerifyOtpRequestDto request)
    {
        try
        {
            var result = await _authService.VerifyOtpAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during OTP verification");
            return StatusCode(500, ResponseDto<UserResponseDto>.ErrorResponse("An error occurred during verification"));
        }
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<ResponseDto<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, ResponseDto<LoginResponseDto>.ErrorResponse("An error occurred during login"));
        }
    }

    /// <summary>
    /// Generate new OTP
    /// </summary>
    [HttpPost("generate-otp")]
    public async Task<ActionResult<ResponseDto<string>>> GenerateOtp([FromBody] GenerateOtpRequestDto request)
    {
        try
        {
            var result = await _authService.GenerateOtpAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating OTP");
            return StatusCode(500, ResponseDto<string>.ErrorResponse("An error occurred while generating OTP"));
        }
    }
}
