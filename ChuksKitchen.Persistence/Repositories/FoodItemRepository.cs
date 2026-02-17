using ChuksKitchen.Application.Repositories.Interfaces;
using ChuksKitchen.Domain.Entities;
using ChuksKitchen.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ChuksKitchen.Persistence.Repositories;

public class FoodItemRepository : Repository<FoodItem>, IFoodItemRepository
{
    public FoodItemRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<FoodItem>> GetAvailableItemsAsync()
    {
        return await _context.FoodItems
            .Where(f => f.IsAvailable && f.StockQuantity > 0)
            .OrderBy(f => f.Category)
            .ThenBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<FoodItem>> GetByCategoryAsync(string category)
    {
        return await _context.FoodItems
            .Where(f => f.Category == category && f.IsAvailable)
            .OrderBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<FoodItem?> GetWithReviewsAsync(int id)
    {
        return await _context.FoodItems
            .Include(f => f.OrderItems)
            .FirstOrDefaultAsync(f => f.Id == id);
    }
}
