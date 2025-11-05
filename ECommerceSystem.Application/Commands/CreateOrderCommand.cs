using ECommerceSystem.Application.DTOs;
using MediatR;

namespace ECommerceSystem.Application.Commands;

public class CreateOrderCommand : IRequest<int>
{
    public string UserId { get; set; } = string.Empty;
    public CreateOrderDto Order { get; set; } = null!;
}
