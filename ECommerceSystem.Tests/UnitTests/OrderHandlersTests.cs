using ECommerceSystem.Application.Commands;
using ECommerceSystem.Application.Handlers;
using ECommerceSystem.Application.Queries;
using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace ECommerceSystem.Tests.UnitTests;

[TestFixture]
public class OrderHandlersTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private CreateOrderHandler _createHandler;
    private UpdateOrderStatusHandler _updateStatusHandler;
    private GetOrdersByUserHandler _getOrdersByUserHandler;
    private GetOrderByIdHandler _getOrderByIdHandler;

    [SetUp]
    public void Setup()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _createHandler = new CreateOrderHandler(_unitOfWorkMock.Object, null); // We'll mock mapper separately
        _updateStatusHandler = new UpdateOrderStatusHandler(_unitOfWorkMock.Object);
        _getOrdersByUserHandler = new GetOrdersByUserHandler(_unitOfWorkMock.Object, null);
        _getOrderByIdHandler = new GetOrderByIdHandler(_unitOfWorkMock.Object, null);
    }

    [Test]
    public async Task CreateOrderHandler_ShouldCreateOrderSuccessfully()
    {
        // Arrange
        var orderDto = new Application.DTOs.CreateOrderDto
        {
            PaymentMethod = "PIX",
            OrderItems = new List<Application.DTOs.CreateOrderItemDto>
            {
                new Application.DTOs.CreateOrderItemDto { ProductId = 1, Quantity = 2 }
            }
        };

        var command = new CreateOrderCommand { UserId = "user1", Order = orderDto };
        var product = new Product { Id = 1, Name = "Test Product", Price = 50.00m, StockQuantity = 10 };

        _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(1)).ReturnsAsync(product);
        _unitOfWorkMock.Setup(u => u.Orders.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _createHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.Orders.AddAsync(It.IsAny<Order>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task CreateOrderHandler_ShouldThrowException_WhenProductNotFound()
    {
        // Arrange
        var orderDto = new Application.DTOs.CreateOrderDto
        {
            PaymentMethod = "PIX",
            OrderItems = new List<Application.DTOs.CreateOrderItemDto>
            {
                new Application.DTOs.CreateOrderItemDto { ProductId = 999, Quantity = 1 }
            }
        };

        var command = new CreateOrderCommand { UserId = "user1", Order = orderDto };

        _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(999)).ReturnsAsync((Product?)null);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);

        // Act & Assert
        await _createHandler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Product 999 not found");

        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }

    [Test]
    public async Task CreateOrderHandler_ShouldThrowException_WhenInsufficientStock()
    {
        // Arrange
        var orderDto = new Application.DTOs.CreateOrderDto
        {
            PaymentMethod = "PIX",
            OrderItems = new List<Application.DTOs.CreateOrderItemDto>
            {
                new Application.DTOs.CreateOrderItemDto { ProductId = 1, Quantity = 20 }
            }
        };

        var command = new CreateOrderCommand { UserId = "user1", Order = orderDto };
        var product = new Product { Id = 1, Name = "Test Product", Price = 50.00m, StockQuantity = 10 };

        _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(1)).ReturnsAsync(product);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);

        // Act & Assert
        await _createHandler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Insufficient stock for product Test Product");

        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateOrderStatusHandler_ShouldUpdateStatusSuccessfully()
    {
        // Arrange
        var order = new Order { Id = 1, Status = OrderStatus.Pending };
        var command = new UpdateOrderStatusCommand { OrderId = 1, Status = OrderStatus.Confirmed };

        _unitOfWorkMock.Setup(u => u.Orders.GetByIdAsync(1)).ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _updateStatusHandler.Handle(command, CancellationToken.None);

        // Assert
        order.Status.Should().Be(OrderStatus.Confirmed);
        order.IsPaid.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.Orders.UpdateAsync(order), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateOrderStatusHandler_ShouldThrowException_WhenOrderNotFound()
    {
        // Arrange
        var command = new UpdateOrderStatusCommand { OrderId = 999, Status = OrderStatus.Shipped };

        _unitOfWorkMock.Setup(u => u.Orders.GetByIdAsync(999)).ReturnsAsync((Order?)null);

        // Act & Assert
        await _updateStatusHandler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Order not found");
    }

    [Test]
    public async Task GetOrderByIdHandler_ShouldReturnOrder_WhenExists()
    {
        // Arrange
        var order = new Order { Id = 1, UserId = "user1", Status = OrderStatus.Pending };
        _unitOfWorkMock.Setup(u => u.Orders.GetByIdAsync(1)).ReturnsAsync(order);

        var query = new GetOrderByIdQuery { Id = 1 };

        // Act
        var result = await _getOrderByIdHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Test]
    public async Task GetOrderByIdHandler_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Orders.GetByIdAsync(999)).ReturnsAsync((Order?)null);

        var query = new GetOrderByIdQuery { Id = 999 };

        // Act
        var result = await _getOrderByIdHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
