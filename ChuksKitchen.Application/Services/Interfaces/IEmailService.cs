namespace ChuksKitchen.Application.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendOtpEmailAsync(string email, string otp);
    Task<bool> SendOrderConfirmationEmailAsync(string email, string orderNumber);
    Task<bool> SendOrderStatusUpdateEmailAsync(string email, string orderNumber, string status);
}
