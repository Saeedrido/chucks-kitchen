namespace ChuksKitchen.Application.Services.Interfaces;

public interface IReferralCodeService
{
    Task<string> GenerateUniqueReferralCodeAsync();
}
