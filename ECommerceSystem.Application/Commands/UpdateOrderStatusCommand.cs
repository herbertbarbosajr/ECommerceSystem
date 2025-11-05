using ECommerceSystem.Core.Entities;
using MediatR;

namespace ECommerceSystem.Application.Commands;

public class UpdateOrderStatusCommand : IRequest
{
    public int OrderId { get; set; }
    public OrderStatus Status { get; set; }
}
