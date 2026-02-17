using ChuksKitchen.Application.DTOs.Responses;

namespace ChuksKitchen.Application.Services.Interfaces;

/// <summary>
/// User service interface for user-related business logic
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get user by referral code
    /// </summary>
    Task<ResponseDto<UserReferralDto>> GetUserByReferralCodeAsync(string referralCode);

    /// <summary>
    /// Get current user profile by ID
    /// </summary>
    Task<ResponseDto<UserProfileDto>> GetUserByIdAsync(int userId);
}
