using ECommerceSystem.Application.DTOs;
using MediatR;

namespace ECommerceSystem.Application.Commands;

public class CreateProductCommand : IRequest<int>
{
    public CreateProductDto Product { get; set; } = null!;
}
