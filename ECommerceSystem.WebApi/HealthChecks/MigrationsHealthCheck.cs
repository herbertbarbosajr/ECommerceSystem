using ECommerceSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ECommerceSystem.WebApi.HealthChecks;

public class MigrationsHealthCheck : IHealthCheck
{
    private readonly ECommerceDbContext _context;
    
    public MigrationsHealthCheck(ECommerceDbContext context)
    {
        _context = context;
    }
    
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var pending = _context.Database.GetPendingMigrations();
        return Task.FromResult(!pending.Any() 
            ? HealthCheckResult.Healthy("All migrations are applied")
            : HealthCheckResult.Degraded($"Pending migrations: {string.Join(", ", pending)}"));
    }
}