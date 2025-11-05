using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Common;

namespace ECommerceSystem.Core.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetActiveAsync();
    Task<(IEnumerable<Product> Items, int TotalCount)> GetAsync(ISpecification<Product> spec, PaginationParams paginationParams);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task UpdateStockAsync(int productId, int quantity);
}
