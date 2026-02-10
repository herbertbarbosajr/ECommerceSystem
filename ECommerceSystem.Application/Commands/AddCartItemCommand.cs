using ECommerceSystem.Application.DTOs;
using MediatR;

namespace ECommerceSystem.Application.Commands;

public class AddCartItemCommand : IRequest<int>
{
    public string UserId { get; set; } = string.Empty;
    public AddCartItemDto CartItem { get; set; } = null!;
}
