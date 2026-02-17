using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using ChuksKitchen.Application.DTOs.Requests;
using ChuksKitchen.Application.DTOs.Responses;
using ChuksKitchen.Application.Repositories.Interfaces;
using ChuksKitchen.Application.Services.Interfaces;
using ChuksKitchen.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ChuksKitchen.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IReferralCodeService _referralCodeService;

    public AuthService(IUserRepository userRepository, ILogger<AuthService> logger, IConfiguration configuration, IReferralCodeService referralCodeService)
    {
        _userRepository = userRepository;
        _logger = logger;
        _configuration = configuration;
        _referralCodeService = referralCodeService;
    }

    public async Task<ResponseDto<UserResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            // Business Rule: Check for duplicate email or phone
            var existingUser = await _userRepository.GetByEmailOrPhoneAsync(request.Email, request.Phone ?? "");

            if (existingUser != null)
            {
                if (existingUser.Email == request.Email)
                    return ResponseDto<UserResponseDto>.ErrorResponse("Email already registered");

                if (existingUser.Phone == request.Phone)
                    return ResponseDto<UserResponseDto>.ErrorResponse("Phone number already registered");
            }

            // Business Rule: Validate referral code if provided
            int? referrerId = null;
            if (!string.IsNullOrWhiteSpace(request.ReferralCode))
            {
                var referrer = await _userRepository.GetByReferralCodeAsync(request.ReferralCode);
                if (referrer == null)
                    return ResponseDto<UserResponseDto>.ErrorResponse("Invalid referral code");
                referrerId = referrer.Id;
            }

            // Generate unique referral code for the new user
            var referralCode = await _referralCodeService.GenerateUniqueReferralCodeAsync();

            var user = new User
            {
                Email = request.Email,
                Phone = request.Phone,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = HashPassword(request.Password),
                RegistrationMethod = request.RegistrationMethod,
                ReferrerId = referrerId,
                IsVerified = false,
                Role = request.Role,
                ReferralCode = referralCode
            };

            var createdUser = await _userRepository.AddAsync(user);

            var otpResponse = await GenerateOtpAsync(new GenerateOtpRequestDto { EmailOrPhone = request.Email });

            if (!otpResponse.Success)
            {
                _logger.LogWarning($"OTP generation failed for user {createdUser.Id}");
            }

            var response = MapToUserResponse(createdUser);

            return ResponseDto<UserResponseDto>.SuccessResponse(response, "Registration successful. Please verify your account with the OTP sent to your email/phone.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return ResponseDto<UserResponseDto>.ErrorResponse("Registration failed. Please try again.");
        }
    }

    public async Task<ResponseDto<UserResponseDto>> VerifyOtpAsync(VerifyOtpRequestDto request)
    {
        try
        {
            var user = await _userRepository.GetByEmailOrPhoneAsync(request.EmailOrPhone, request.EmailOrPhone);

            if (user == null)
                return ResponseDto<UserResponseDto>.ErrorResponse("User not found");

            // Business Rule: Check if already verified
            if (user.IsVerified)
                return ResponseDto<UserResponseDto>.ErrorResponse("Account already verified");

            user.IsVerified = true;
            user.OtpCode = null;
            user.FailedOtpAttempts = 0;
            await _userRepository.Update(user);

            var response = MapToUserResponse(user);
            return ResponseDto<UserResponseDto>.SuccessResponse(response, "Account verified successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during OTP verification");
            return ResponseDto<UserResponseDto>.ErrorResponse("Verification failed. Please try again.");
        }
    }

    public async Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var user = await _userRepository.GetByEmailOrPhoneAsync(request.EmailOrPhone, request.EmailOrPhone);

            if (user == null)
                return ResponseDto<LoginResponseDto>.ErrorResponse("Invalid credentials");

            if (!VerifyPassword(request.Password, user.PasswordHash))
                return ResponseDto<LoginResponseDto>.ErrorResponse("Invalid credentials");

            if (!user.IsVerified)
                return ResponseDto<LoginResponseDto>.ErrorResponse("Please verify your account before logging in");

            var token = GenerateJwtToken(user);
            var userResponse = MapToUserResponse(user);
            var expiration = DateTime.UtcNow.AddHours(24); // Token valid for 24 hours

            var loginResponse = new LoginResponseDto
            {
                User = userResponse,
                Token = token,
                Expiration = expiration
            };

            return ResponseDto<LoginResponseDto>.SuccessResponse(loginResponse, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return ResponseDto<LoginResponseDto>.ErrorResponse("Login failed. Please try again.");
        }
    }

    public async Task<ResponseDto<string>> GenerateOtpAsync(GenerateOtpRequestDto request)
    {
        try
        {
            var user = await _userRepository.GetByEmailOrPhoneAsync(request.EmailOrPhone, request.EmailOrPhone);

            if (user == null)
                return ResponseDto<string>.ErrorResponse("User not found");

            // Business Rule: Generate 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();
            user.OtpCode = otp;
            user.OtpGeneratedAt = DateTime.UtcNow;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
            user.FailedOtpAttempts = 0;

            await _userRepository.Update(user);

            // In production, this would send email/SMS
            // For now, returning OTP in response (development only)
            return ResponseDto<string>.SuccessResponse(otp, $"OTP generated successfully. Valid for 10 minutes. Your OTP is: {otp}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating OTP");
            return ResponseDto<string>.ErrorResponse("Failed to generate OTP. Please try again.");
        }
    }

    private string HashPassword(string password)
    {
        // BCrypt automatically handles salt and secure hashing
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hash)
    {
        // BCrypt verifies password against hash
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "ChuksKitchenSecretKeyForJWTTokenGeneration123!@#";
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "ChuksKitchenAPI";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "ChuksKitchenClient";

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("userId", user.Id.ToString()),
            new Claim("role", user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24), // Token expires in 24 hours
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private UserResponseDto MapToUserResponse(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Phone = user.Phone,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsVerified = user.IsVerified,
            Role = user.Role,
            ReferralCode = user.ReferralCode,
            FullName = $"{user.FirstName} {user.LastName}"
        };
    }
}
