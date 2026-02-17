using ChuksKitchen.Application.Repositories.Interfaces;
using ChuksKitchen.Application.Services.Interfaces;

namespace ChuksKitchen.Application.Services;

public class ReferralCodeService : IReferralCodeService
{
    private readonly IUserRepository _userRepository;
    private readonly Random _random = new();

    public ReferralCodeService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string> GenerateUniqueReferralCodeAsync()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // No I, O, 0, 1 to avoid confusion
        const int codeLength = 6;
        const string prefix = "CK";

        string code;
        bool isUnique;

        // Keep generating until we find a unique code
        do
        {
            var randomChars = new char[codeLength];
            for (int i = 0; i < codeLength; i++)
            {
                randomChars[i] = chars[_random.Next(chars.Length)];
            }

            code = $"{prefix}-{new string(randomChars)}";

            // Check if code already exists
            var existingUser = await _userRepository.GetByReferralCodeAsync(code);
            isUnique = existingUser == null;

        } while (!isUnique);

        return code;
    }
}
