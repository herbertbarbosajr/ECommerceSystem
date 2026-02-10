using ECommerceSystem.Application.DTOs;
using MediatR;

namespace ECommerceSystem.Application.Commands;

public class UpdateCartItemCommand : IRequest
{
    public int CartItemId { get; set; }
    public UpdateCartItemDto CartItem { get; set; } = null!;
}
