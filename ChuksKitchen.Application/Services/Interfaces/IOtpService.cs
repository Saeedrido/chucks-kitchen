namespace ChuksKitchen.Application.Services.Interfaces;

public interface IOtpService
{
    string GenerateOtp();
    bool ValidateOtp(string inputOtp, string storedOtp, DateTime expiry);
}
