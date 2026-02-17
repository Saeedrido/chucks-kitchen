using ChuksKitchen.Domain.Entities;

namespace ChuksKitchen.Application.Repositories.Interfaces;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(int userId);
    Task<Cart?> GetWithItemsAsync(int cartId);
    Task<Cart?> GetOrCreateCartAsync(int userId);
}
