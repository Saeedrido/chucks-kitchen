namespace ChuksKitchen.Application.Services.Interfaces;

public interface ISmsService
{
    Task<bool> SendOtpSmsAsync(string phoneNumber, string otp);
    Task<bool> SendOrderStatusSmsAsync(string phoneNumber, string orderNumber, string status);
}
