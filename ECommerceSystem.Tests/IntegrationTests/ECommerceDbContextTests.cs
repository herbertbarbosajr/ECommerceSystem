using ECommerceSystem.Core.Entities;
using ECommerceSystem.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSystem.Tests.IntegrationTests;

[TestFixture]
public class ECommerceDbContextTests
{
    private ECommerceDbContext _context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ECommerceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ECommerceDbContext(options);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task ShouldAddAndRetrieveProduct()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 10,
            IsActive = true
        };

        // Act
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var retrievedProduct = await _context.Products.FindAsync(product.Id);

        // Assert
        retrievedProduct.Should().NotBeNull();
        retrievedProduct.Name.Should().Be("Test Product");
        retrievedProduct.Description.Should().Be("Test Description");
        retrievedProduct.Price.Should().Be(99.99m);
        retrievedProduct.StockQuantity.Should().Be(10);
        retrievedProduct.IsActive.Should().BeTrue();
    }

    [Test]
    public async Task ShouldAddAndRetrieveOrderWithItems()
    {
        // Arrange
        var user = new User
        {
            Id = "test-user",
            UserName = "test@example.com",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            IsAdmin = false
        };

        var product = new Product
        {
            Name = "Test Product",
            Price = 50.00m,
            StockQuantity = 10,
            IsActive = true
        };

        var order = new Order
        {
            UserId = user.Id,
            User = user,
            PaymentMethod = "PIX",
            Status = OrderStatus.Pending,
            TotalAmount = 100.00m,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    Product = product,
                    Quantity = 2,
                    UnitPrice = 50.00m
                }
            }
        };

        // Act
        await _context.Users.AddAsync(user);
        await _context.Products.AddAsync(product);
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        var retrievedOrder = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == order.Id);

        // Assert
        retrievedOrder.Should().NotBeNull();
        retrievedOrder.UserId.Should().Be(user.Id);
        retrievedOrder.PaymentMethod.Should().Be("PIX");
        retrievedOrder.Status.Should().Be(OrderStatus.Pending);
        retrievedOrder.TotalAmount.Should().Be(100.00m);
        retrievedOrder.OrderItems.Should().HaveCount(1);

        var orderItem = retrievedOrder.OrderItems.First();
        orderItem.Product.Name.Should().Be("Test Product");
        orderItem.Quantity.Should().Be(2);
        orderItem.UnitPrice.Should().Be(50.00m);
        orderItem.TotalPrice.Should().Be(100.00m);
    }

    [Test]
    public async Task ShouldUpdateProductStock()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            StockQuantity = 10,
            IsActive = true
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        product.StockQuantity = 5;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        var updatedProduct = await _context.Products.FindAsync(product.Id);

        // Assert
        updatedProduct.StockQuantity.Should().Be(5);
    }

    [Test]
    public async Task ShouldFilterActiveProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Name = "Active Product 1", IsActive = true },
            new Product { Name = "Inactive Product", IsActive = false },
            new Product { Name = "Active Product 2", IsActive = true }
        };

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Act
        var activeProducts = await _context.Products.Where(p => p.IsActive).ToListAsync();

        // Assert
        activeProducts.Should().HaveCount(2);
        activeProducts.All(p => p.IsActive).Should().BeTrue();
    }

    [Test]
    public async Task ShouldRetrieveOrdersByUser()
    {
        // Arrange
        var user1 = new User { Id = "user1", UserName = "user1@example.com", Email = "user1@example.com" };
        var user2 = new User { Id = "user2", UserName = "user2@example.com", Email = "user2@example.com" };

        var orders = new List<Order>
        {
            new Order { UserId = user1.Id, User = user1, Status = OrderStatus.Pending },
            new Order { UserId = user1.Id, User = user1, Status = OrderStatus.Confirmed },
            new Order { UserId = user2.Id, User = user2, Status = OrderStatus.Pending }
        };

        await _context.Users.AddRangeAsync(user1, user2);
        await _context.Orders.AddRangeAsync(orders);
        await _context.SaveChangesAsync();

        // Act
        var user1Orders = await _context.Orders.Where(o => o.UserId == user1.Id).ToListAsync();

        // Assert
        user1Orders.Should().HaveCount(2);
        user1Orders.All(o => o.UserId == user1.Id).Should().BeTrue();
    }
}
