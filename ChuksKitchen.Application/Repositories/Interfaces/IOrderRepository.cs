using ChuksKitchen.Domain.Entities;

namespace ChuksKitchen.Application.Repositories.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<Order?> GetWithItemsAsync(int orderId);
    Task<Order?> GetUserOrderAsync(int userId, int orderId);
    Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
}
