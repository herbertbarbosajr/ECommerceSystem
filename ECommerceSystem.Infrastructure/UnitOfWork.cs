using ECommerceSystem.Core.Interfaces;
using ECommerceSystem.Infrastructure.Data;
using ECommerceSystem.Infrastructure.Repositories;

namespace ECommerceSystem.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ECommerceDbContext _context;
    private IProductRepository? _products;
    private IOrderRepository? _orders;

    public UnitOfWork(ECommerceDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
