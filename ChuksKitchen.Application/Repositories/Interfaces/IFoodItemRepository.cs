using ChuksKitchen.Domain.Entities;

namespace ChuksKitchen.Application.Repositories.Interfaces;

public interface IFoodItemRepository : IRepository<FoodItem>
{
    Task<IEnumerable<FoodItem>> GetAvailableItemsAsync();
    Task<IEnumerable<FoodItem>> GetByCategoryAsync(string category);
    Task<FoodItem?> GetWithReviewsAsync(int id);
}
