using ECommerceSystem.Application.Commands;
using ECommerceSystem.Application.Handlers;
using ECommerceSystem.Application.Queries;
using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Interfaces;
using FluentAssertions;
using Moq;
using AutoMapper;

namespace ECommerceSystem.Tests.UnitTests;

[TestFixture]
public class ProductHandlersTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private CreateProductHandler _createHandler;
    private UpdateProductHandler _updateHandler;
    private DeleteProductHandler _deleteHandler;
    private GetProductsHandler _getProductsHandler;
    private GetProductByIdHandler _getProductByIdHandler;

    [SetUp]
    public void Setup()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        _createHandler = new CreateProductHandler(_unitOfWorkMock.Object, mapperMock.Object);
        _updateHandler = new UpdateProductHandler(_unitOfWorkMock.Object, mapperMock.Object);
        _deleteHandler = new DeleteProductHandler(_unitOfWorkMock.Object);
        _getProductsHandler = new GetProductsHandler(_unitOfWorkMock.Object, mapperMock.Object);
        _getProductByIdHandler = new GetProductByIdHandler(_unitOfWorkMock.Object, mapperMock.Object);
    }

    [Test]
    public async Task CreateProductHandler_ShouldCreateProductSuccessfully()
    {
        // Arrange
        var productDto = new Application.DTOs.CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 10
        };

        var command = new CreateProductCommand { Product = productDto };
        var product = new Product
        {
            Id = 1,
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            StockQuantity = productDto.StockQuantity,
            IsActive = true
        };

        _unitOfWorkMock.Setup(u => u.Products.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _createHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);
        _unitOfWorkMock.Verify(u => u.Products.AddAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task GetProductsHandler_ShouldReturnActiveProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", IsActive = true },
            new Product { Id = 2, Name = "Product 2", IsActive = false },
            new Product { Id = 3, Name = "Product 3", IsActive = true }
        };

        _unitOfWorkMock.Setup(u => u.Products.GetActiveAsync()).ReturnsAsync(products.Where(p => p.IsActive));

        var query = new GetProductsQuery { OnlyActive = true };

        // Act
        var result = await _getProductsHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.All(p => p.IsActive).Should().BeTrue();
    }

    [Test]
    public async Task GetProductByIdHandler_ShouldReturnProduct_WhenExists()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product", IsActive = true };
        _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(1)).ReturnsAsync(product);

        var query = new GetProductByIdQuery { Id = 1 };

        // Act
        var result = await _getProductByIdHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Test Product");
    }

    [Test]
    public async Task GetProductByIdHandler_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(999)).ReturnsAsync((Product?)null);

        var query = new GetProductByIdQuery { Id = 999 };

        // Act
        var result = await _getProductByIdHandler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task UpdateProductHandler_ShouldUpdateProductSuccessfully()
    {
        // Arrange
        var existingProduct = new Product { Id = 1, Name = "Old Name", Price = 50.00m };
        var updateDto = new Application.DTOs.UpdateProductDto
        {
            Name = "New Name",
            Price = 75.00m,
            StockQuantity = 20
        };

        _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(1)).ReturnsAsync(existingProduct);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var command = new UpdateProductCommand { Id = 1, Product = updateDto };

        // Act
        await _updateHandler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Products.UpdateAsync(It.Is<Product>(p =>
            p.Id == 1 &&
            p.Name == "New Name" &&
            p.Price == 75.00m &&
            p.StockQuantity == 20)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteProductHandler_ShouldDeleteProductSuccessfully()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product" };
        _unitOfWorkMock.Setup(u => u.Products.GetByIdAsync(1)).ReturnsAsync(product);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var command = new DeleteProductCommand { Id = 1 };

        // Act
        await _deleteHandler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Products.DeleteAsync(1), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
