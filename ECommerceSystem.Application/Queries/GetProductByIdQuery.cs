using ECommerceSystem.Application.DTOs;
using MediatR;

namespace ECommerceSystem.Application.Queries;

public class GetProductByIdQuery : IRequest<ProductDto?>
{
    public int Id { get; set; }
}
