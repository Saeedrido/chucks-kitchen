using ChuksKitchen.Application.DTOs.Requests;
using ChuksKitchen.Application.DTOs.Responses;
using ChuksKitchen.Domain.Entities;

namespace ChuksKitchen.Application.Services.Interfaces;

public interface IAuthService
{
    Task<ResponseDto<UserResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ResponseDto<UserResponseDto>> VerifyOtpAsync(VerifyOtpRequestDto request);
    Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ResponseDto<string>> GenerateOtpAsync(GenerateOtpRequestDto request);
    string GenerateJwtToken(User user);
}
