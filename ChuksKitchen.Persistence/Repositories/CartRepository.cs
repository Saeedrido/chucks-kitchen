using ChuksKitchen.Application.Repositories.Interfaces;
using ChuksKitchen.Domain.Entities;
using ChuksKitchen.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ChuksKitchen.Persistence.Repositories;

public class CartRepository : Repository<Cart>, ICartRepository
{
    public CartRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Cart?> GetByUserIdAsync(int userId)
    {
        return await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.FoodItem)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart?> GetWithItemsAsync(int cartId)
    {
        return await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.FoodItem)
            .FirstOrDefaultAsync(c => c.Id == cartId);
    }

    public async Task<Cart?> GetOrCreateCartAsync(int userId)
    {
        var cart = await GetByUserIdAsync(userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            await AddAsync(cart);
        }

        return cart;
    }
}
