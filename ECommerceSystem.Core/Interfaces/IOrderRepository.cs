using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Common;

namespace ECommerceSystem.Core.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<(IEnumerable<Order> Items, int TotalCount)> GetAsync(ISpecification<Order> spec, PaginationParams paginationParams);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
