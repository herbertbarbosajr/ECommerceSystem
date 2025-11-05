using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Interfaces;
using ECommerceSystem.Infrastructure.Data;
using ECommerceSystem.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSystem.Tests.IntegrationTests;

[TestFixture]
public class RepositoryTests
{
    private ECommerceDbContext _context;
    private IProductRepository _productRepository;
    private IOrderRepository _orderRepository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ECommerceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ECommerceDbContext(options);
        _productRepository = new ProductRepository(_context);
        _orderRepository = new OrderRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task ProductRepository_ShouldAddAndRetrieveProduct()
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
        await _productRepository.AddAsync(product);
        await _context.SaveChangesAsync();

        var retrievedProduct = await _productRepository.GetByIdAsync(product.Id);

        // Assert
        retrievedProduct.Should().NotBeNull();
        retrievedProduct.Name.Should().Be("Test Product");
        retrievedProduct.StockQuantity.Should().Be(10);
    }

    [Test]
    public async Task ProductRepository_ShouldUpdateStock()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            StockQuantity = 20,
            IsActive = true
        };

        await _productRepository.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        await _productRepository.UpdateStockAsync(product.Id, 15);
        await _context.SaveChangesAsync();

        var updatedProduct = await _productRepository.GetByIdAsync(product.Id);

        // Assert
        updatedProduct.StockQuantity.Should().Be(15);
    }

    [Test]
    public async Task ProductRepository_ShouldGetActiveProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Name = "Active 1", IsActive = true },
            new Product { Name = "Inactive", IsActive = false },
            new Product { Name = "Active 2", IsActive = true }
        };

        foreach (var product in products)
        {
            await _productRepository.AddAsync(product);
        }
        await _context.SaveChangesAsync();

        // Act
        var activeProducts = await _productRepository.GetActiveAsync();

        // Assert
        activeProducts.Should().HaveCount(2);
        activeProducts.All(p => p.IsActive).Should().BeTrue();
    }

    [Test]
    public async Task ProductRepository_ShouldDeleteProduct()
    {
        // Arrange
        var product = new Product { Name = "Test Product", IsActive = true };
        await _productRepository.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        await _productRepository.DeleteAsync(product.Id);
        await _context.SaveChangesAsync();

        var deletedProduct = await _productRepository.GetByIdAsync(product.Id);

        // Assert
        deletedProduct.Should().BeNull();
    }

    [Test]
    public async Task OrderRepository_ShouldAddAndRetrieveOrder()
    {
        // Arrange
        var user = new User
        {
            Id = "test-user",
            UserName = "test@example.com",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User"
        };

        var product = new Product { Name = "Test Product", Price = 50.00m, IsActive = true };

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

        await _context.Users.AddAsync(user);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        await _orderRepository.AddAsync(order);
        await _context.SaveChangesAsync();

        var retrievedOrder = await _orderRepository.GetByIdAsync(order.Id);

        // Assert
        retrievedOrder.Should().NotBeNull();
        retrievedOrder.UserId.Should().Be(user.Id);
        retrievedOrder.Status.Should().Be(OrderStatus.Pending);
        retrievedOrder.OrderItems.Should().HaveCount(1);
    }

    [Test]
    public async Task OrderRepository_ShouldGetOrdersByUser()
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
        foreach (var order in orders)
        {
            await _orderRepository.AddAsync(order);
        }
        await _context.SaveChangesAsync();

        // Act
        var user1Orders = await _orderRepository.GetByUserIdAsync(user1.Id);

        // Assert
        user1Orders.Should().HaveCount(2);
        user1Orders.All(o => o.UserId == user1.Id).Should().BeTrue();
    }

    [Test]
    public async Task OrderRepository_ShouldUpdateOrder()
    {
        // Arrange
        var user = new User { Id = "user", UserName = "user@example.com", Email = "user@example.com" };
        var order = new Order { UserId = user.Id, User = user, Status = OrderStatus.Pending };

        await _context.Users.AddAsync(user);
        await _orderRepository.AddAsync(order);
        await _context.SaveChangesAsync();

        // Act
        order.Status = OrderStatus.Shipped;
        await _orderRepository.UpdateAsync(order);
        await _context.SaveChangesAsync();

        var updatedOrder = await _orderRepository.GetByIdAsync(order.Id);

        // Assert
        updatedOrder.Status.Should().Be(OrderStatus.Shipped);
    }
}
