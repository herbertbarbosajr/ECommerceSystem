using MediatR;

namespace ECommerceSystem.Application.Commands;

public class DeleteProductCommand : IRequest
{
    public int Id { get; set; }
}
