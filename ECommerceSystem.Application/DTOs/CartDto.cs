using ECommerceSystem.Application.DTOs;

namespace ECommerceSystem.Application.DTOs;

public class CartDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    public decimal TotalAmount => Items.Sum(item => item.TotalPrice);
}

public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}

public class AddCartItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateCartItemDto
{
    public int Quantity { get; set; }
}
