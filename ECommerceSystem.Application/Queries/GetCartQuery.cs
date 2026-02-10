using ECommerceSystem.Application.DTOs;
using MediatR;

namespace ECommerceSystem.Application.Queries;

public class GetCartQuery : IRequest<CartDto?>
{
    public string UserId { get; set; } = string.Empty;
}
