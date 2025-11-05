using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Interfaces;
using ECommerceSystem.Core.Common;
using ECommerceSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSystem.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ECommerceDbContext _context;

    public ProductRepository(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetActiveAsync()
    {
        return await _context.Products.Where(p => p.IsActive).ToListAsync();
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetAsync(ISpecification<Product> spec, PaginationParams paginationParams)
    {
        var query = ApplySpecification(spec);

        var totalCount = await query.CountAsync();

        if (spec.IsPagingEnabled)
        {
            query = query.Skip(spec.Skip).Take(spec.Take);
        }
        else
        {
            query = query.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                        .Take(paginationParams.PageSize);
        }

        var items = await query.ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await GetByIdAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task UpdateStockAsync(int productId, int quantity)
    {
        var product = await GetByIdAsync(productId);
        if (product != null)
        {
            product.StockQuantity = quantity;
            _context.Products.Update(product);
        }
    }

    private IQueryable<Product> ApplySpecification(ISpecification<Product> spec)
    {
        var query = _context.Products.AsQueryable();

        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }

        if (spec.OrderBy != null)
        {
            query = query.OrderBy(spec.OrderBy);
        }
        else if (spec.OrderByDescending != null)
        {
            query = query.OrderByDescending(spec.OrderByDescending);
        }

        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

        foreach (var includeString in spec.IncludeStrings)
        {
            query = query.Include(includeString);
        }

        return query;
    }
}
