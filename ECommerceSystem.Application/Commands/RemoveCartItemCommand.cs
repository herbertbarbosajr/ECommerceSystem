using MediatR;

namespace ECommerceSystem.Application.Commands;

public class RemoveCartItemCommand : IRequest
{
    public int CartItemId { get; set; }
}
