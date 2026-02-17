using ChuksKitchen.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ChuksKitchen.Infrastructure.Services;

public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;

    public SmsService(ILogger<SmsService> logger)
    {
        _logger = logger;
    }

    public Task<bool> SendOtpSmsAsync(string phoneNumber, string otp)
    {
        try
        {
            // In production, integrate with real SMS service (Twilio, AWS SNS, etc.)
            _logger.LogInformation("OTP SMS Simulation");
            _logger.LogInformation("To: {PhoneNumber}", phoneNumber);
            _logger.LogInformation("Message: Your Chuks Kitchen verification code is: {Otp}. Valid for 10 minutes.", otp);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending OTP SMS to {PhoneNumber}", phoneNumber);
            return Task.FromResult(false);
        }
    }

    public Task<bool> SendOrderStatusSmsAsync(string phoneNumber, string orderNumber, string status)
    {
        try
        {
            _logger.LogInformation("Order Status SMS Simulation");
            _logger.LogInformation("To: {PhoneNumber}", phoneNumber);
            _logger.LogInformation("Message: Order {OrderNumber} status updated to: {Status}", orderNumber, status);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending order status SMS to {PhoneNumber}", phoneNumber);
            return Task.FromResult(false);
        }
    }
}
