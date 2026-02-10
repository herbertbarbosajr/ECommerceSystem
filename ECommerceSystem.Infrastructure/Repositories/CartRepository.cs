using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Interfaces;
using ECommerceSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSystem.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ECommerceDbContext _context;

    public CartRepository(ECommerceDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetCartByUserIdAsync(string userId)
    {
        return await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart> CreateCartAsync(Cart cart)
    {
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId)
    {
        return await _context.CartItems
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
    }

    public async Task<CartItem> AddCartItemAsync(CartItem cartItem)
    {
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
        return cartItem;
    }

    public async Task UpdateCartItemAsync(CartItem cartItem)
    {
        _context.CartItems.Update(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveCartItemAsync(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task ClearCartAsync(int cartId)
    {
        var cartItems = await _context.CartItems
            .Where(ci => ci.CartId == cartId)
            .ToListAsync();

        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CartExistsAsync(string userId)
    {
        return await _context.Carts.AnyAsync(c => c.UserId == userId);
    }
}
