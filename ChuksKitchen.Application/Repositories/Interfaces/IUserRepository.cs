using ChuksKitchen.Domain.Entities;

namespace ChuksKitchen.Application.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPhoneAsync(string phone);
    Task<User?> GetByEmailOrPhoneAsync(string email, string phone);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> PhoneExistsAsync(string phone);
    Task<User?> GetWithCartAsync(int userId);
    Task<User?> GetByReferralCodeAsync(string referralCode);
    Task<int> GetReferralCountAsync(int userId);
}
