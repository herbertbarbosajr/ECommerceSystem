using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Interfaces;
using ECommerceSystem.Core.Common;
using ECommerceSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSystem.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ECommerceDbContext _context;

    public OrderRepository(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Order> Items, int TotalCount)> GetAsync(ISpecification<Order> spec, PaginationParams paginationParams)
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

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
    }

    public async Task DeleteAsync(int id)
    {
        var order = await GetByIdAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Orders.AnyAsync(o => o.Id == id);
    }

    private IQueryable<Order> ApplySpecification(ISpecification<Order> spec)
    {
        var query = _context.Orders.AsQueryable();

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
