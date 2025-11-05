using Microsoft.AspNetCore.Identity;

namespace ECommerceSystem.Core.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public string? RefreshToken { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
