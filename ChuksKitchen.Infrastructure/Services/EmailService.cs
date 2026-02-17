using ChuksKitchen.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ChuksKitchen.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task<bool> SendOtpEmailAsync(string email, string otp)
    {
        try
        {
            // In production, integrate with real email service (SendGrid, Mailgun, etc.)
            _logger.LogInformation("OTP Email Simulation");
            _logger.LogInformation("To: {Email}", email);
            _logger.LogInformation("Subject: Verify Your Chuks Kitchen Account");
            _logger.LogInformation("Body: Your verification code is: {Otp}", otp);
            _logger.LogInformation("Valid for: 10 minutes");

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending OTP email to {Email}", email);
            return Task.FromResult(false);
        }
    }

    public Task<bool> SendOrderConfirmationEmailAsync(string email, string orderNumber)
    {
        try
        {
            _logger.LogInformation("Order Confirmation Email Simulation");
            _logger.LogInformation("To: {Email}", email);
            _logger.LogInformation("Subject: Order Confirmed - {OrderNumber}", orderNumber);
            _logger.LogInformation("Body: Your order has been confirmed and is being prepared");

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending order confirmation email to {Email}", email);
            return Task.FromResult(false);
        }
    }

    public Task<bool> SendOrderStatusUpdateEmailAsync(string email, string orderNumber, string status)
    {
        try
        {
            _logger.LogInformation("Order Status Update Email Simulation");
            _logger.LogInformation("To: {Email}", email);
            _logger.LogInformation("Subject: Order Update - {OrderNumber}", orderNumber);
            _logger.LogInformation("Body: Your order status is now: {Status}", status);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending order status update email to {Email}", email);
            return Task.FromResult(false);
        }
    }
}
