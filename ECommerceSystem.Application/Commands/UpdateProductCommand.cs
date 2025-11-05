using ECommerceSystem.Application.DTOs;
using MediatR;

namespace ECommerceSystem.Application.Commands;

public class UpdateProductCommand : IRequest
{
    public int Id { get; set; }
    public UpdateProductDto Product { get; set; } = null!;
}
