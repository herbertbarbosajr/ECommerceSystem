using ECommerceSystem.Application.Commands;
using ECommerceSystem.Application.DTOs;
using ECommerceSystem.Application.Queries;
using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Interfaces;
using MediatR;

namespace ECommerceSystem.Application.Handlers;

public class CartHandlers :
    IRequestHandler<GetCartQuery, CartDto?>,
    IRequestHandler<AddCartItemCommand, int>,
    IRequestHandler<UpdateCartItemCommand>,
    IRequestHandler<RemoveCartItemCommand>
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartHandlers(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto?> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(request.UserId);
        if (cart == null)
            return null;

        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt,
            Items = cart.CartItems.Select(item => new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                ProductImageUrl = item.Product.ImageUrl,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    public async Task<int> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        // Get or create cart
        var cart = await _cartRepository.GetCartByUserIdAsync(request.UserId);
        if (cart == null)
        {
            cart = new Cart { UserId = request.UserId };
            cart = await _cartRepository.CreateCartAsync(cart);
        }

        // Get product and validate stock
        var product = await _productRepository.GetByIdAsync(request.CartItem.ProductId);
        if (product == null || !product.IsActive)
            throw new InvalidOperationException("Produto não encontrado ou inativo");

        if (product.StockQuantity < request.CartItem.Quantity)
            throw new InvalidOperationException("Estoque insuficiente");

        // Check if item already exists in cart
        var existingItem = cart.CartItems.FirstOrDefault(item => item.ProductId == request.CartItem.ProductId);
        if (existingItem != null)
        {
            // Update quantity
            existingItem.Quantity += request.CartItem.Quantity;
            if (product.StockQuantity < existingItem.Quantity)
                throw new InvalidOperationException("Estoque insuficiente para quantidade total");

            await _cartRepository.UpdateCartItemAsync(existingItem);
            return existingItem.Id;
        }
        else
        {
            // Add new item
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = request.CartItem.ProductId,
                Quantity = request.CartItem.Quantity,
                UnitPrice = product.Price
            };

            var addedItem = await _cartRepository.AddCartItemAsync(cartItem);
            return addedItem.Id;
        }
    }

    public async Task Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cartItem = await _cartRepository.GetCartItemByIdAsync(request.CartItemId);
        if (cartItem == null)
            throw new InvalidOperationException("Item do carrinho não encontrado");

        // Validate stock
        var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
        if (product == null || !product.IsActive)
            throw new InvalidOperationException("Produto não encontrado ou inativo");

        if (product.StockQuantity < request.CartItem.Quantity)
            throw new InvalidOperationException("Estoque insuficiente");

        cartItem.Quantity = request.CartItem.Quantity;
        await _cartRepository.UpdateCartItemAsync(cartItem);
    }

    public async Task Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var cartItem = await _cartRepository.GetCartItemByIdAsync(request.CartItemId);
        if (cartItem == null)
            throw new InvalidOperationException("Item do carrinho não encontrado");

        await _cartRepository.RemoveCartItemAsync(cartItem);
    }
}
