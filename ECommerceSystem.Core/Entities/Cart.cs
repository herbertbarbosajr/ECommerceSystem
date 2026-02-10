namespace ECommerceSystem.Core.Entities;

public class Cart
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
