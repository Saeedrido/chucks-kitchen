using ChuksKitchen.Application.DTOs.Responses;
using ChuksKitchen.Application.Repositories.Interfaces;
using ChuksKitchen.Application.Services.Interfaces;
using ChuksKitchen.Domain.Entities;
using ChuksKitchen.Domain.Enums;

namespace ChuksKitchen.Application.Services;

/// <summary>
/// User service containing all user-related business logic
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Get user by referral code with business validation
    /// </summary>
    public async Task<ResponseDto<UserReferralDto>> GetUserByReferralCodeAsync(string referralCode)
    {
        // Business rule: Validate input
        if (string.IsNullOrWhiteSpace(referralCode))
        {
            return ResponseDto<UserReferralDto>.ErrorResponse("Referral code cannot be empty");
        }

        // Data access through repository
        var user = await _userRepository.GetByReferralCodeAsync(referralCode);

        // Business rule: Check if user exists
        if (user == null)
        {
            return ResponseDto<UserReferralDto>.ErrorResponse("Invalid referral code");
        }

        // Business logic: Map Entity to DTO (Service Layer responsibility)
        var userDto = new UserReferralDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            ReferralCode = user.ReferralCode ?? string.Empty
        };

        return ResponseDto<UserReferralDto>.SuccessResponse(userDto, "Referral code found");
    }

    /// <summary>
    /// Get user profile by ID with business logic
    /// </summary>
    public async Task<ResponseDto<UserProfileDto>> GetUserByIdAsync(int userId)
    {
        // Business rule: Validate user ID
        if (userId <= 0)
        {
            return ResponseDto<UserProfileDto>.ErrorResponse("Invalid user ID");
        }

        // Data access through repository
        var user = await _userRepository.GetByIdAsync(userId);

        // Business rule: Check if user exists
        if (user == null)
        {
            return ResponseDto<UserProfileDto>.ErrorResponse("User not found");
        }

        // Business logic: Get referral count (could be complex query)
        int referralCount = await _userRepository.GetReferralCountAsync(user.Id);

        // Business logic: Map Entity to DTO (Service Layer responsibility)
        var userProfile = new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Phone = user.Phone ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            IsVerified = user.IsVerified,
            Role = user.Role,
            RoleDisplay = user.Role == UserRole.Customer ? "Customer" : "Admin",
            ReferralCode = user.ReferralCode ?? string.Empty,
            Address = user.Address ?? string.Empty,
            ReferralCount = referralCount,
            CreatedAt = user.CreatedAt
        };

        return ResponseDto<UserProfileDto>.SuccessResponse(userProfile, "User profile retrieved successfully");
    }
}
