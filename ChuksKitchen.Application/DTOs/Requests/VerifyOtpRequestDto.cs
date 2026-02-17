namespace ChuksKitchen.Application.DTOs.Requests;

public class VerifyOtpRequestDto
{
    public string EmailOrPhone { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}
