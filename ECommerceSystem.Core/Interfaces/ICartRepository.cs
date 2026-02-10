using ECommerceSystem.Core.Entities;

namespace ECommerceSystem.Core.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetCartByUserIdAsync(string userId);
    Task<Cart> CreateCartAsync(Cart cart);
    Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
    Task<CartItem> AddCartItemAsync(CartItem cartItem);
    Task UpdateCartItemAsync(CartItem cartItem);
    Task RemoveCartItemAsync(CartItem cartItem);
    Task ClearCartAsync(int cartId);
    Task<bool> CartExistsAsync(string userId);
}
