using ECommerceSystem.Application.DTOs;
using MediatR;

namespace ECommerceSystem.Application.Queries;

public class GetOrderByIdQuery : IRequest<OrderDto?>
{
    public int Id { get; set; }
}
