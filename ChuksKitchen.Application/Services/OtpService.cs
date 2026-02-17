using ChuksKitchen.Application.Services.Interfaces;

namespace ChuksKitchen.Application.Services;

public class OtpService : IOtpService
{
    public string GenerateOtp()
    {
        // Generate 6-digit OTP
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    public bool ValidateOtp(string inputOtp, string storedOtp, DateTime expiry)
    {
        // Check if OTP matches
        if (inputOtp != storedOtp)
            return false;

        // Check if OTP is expired
        if (DateTime.UtcNow > expiry)
            return false;

        return true;
    }
}
