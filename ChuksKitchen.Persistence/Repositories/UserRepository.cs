using ChuksKitchen.Application.Repositories.Interfaces;
using ChuksKitchen.Domain.Entities;
using ChuksKitchen.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ChuksKitchen.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByPhoneAsync(string phone)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Phone == phone);
    }

    public async Task<User?> GetByEmailOrPhoneAsync(string email, string phone)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email || u.Phone == phone);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email);
    }

    public async Task<bool> PhoneExistsAsync(string phone)
    {
        return await _context.Users
            .AnyAsync(u => u.Phone == phone);
    }

    public async Task<User?> GetWithCartAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Carts)
            .ThenInclude(c => c.CartItems)
            .ThenInclude(ci => ci.FoodItem)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetByReferralCodeAsync(string referralCode)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.ReferralCode == referralCode);
    }

    public async Task<int> GetReferralCountAsync(int userId)
    {
        return await _context.Users
            .CountAsync(u => u.ReferrerId == userId);
    }
}
