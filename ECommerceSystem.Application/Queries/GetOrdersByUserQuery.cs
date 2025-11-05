using ECommerceSystem.Application.DTOs;
using ECommerceSystem.Core.Common;
using ECommerceSystem.Core.Entities;
using MediatR;

namespace ECommerceSystem.Application.Queries;

public class GetOrdersByUserQuery : IRequest<PaginatedResponse<OrderDto>>
{
    public string UserId { get; set; } = string.Empty;
    public OrderStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SortBy { get; set; } = "OrderDate";
    public bool SortDescending { get; set; } = true;
    public PaginationParams PaginationParams { get; set; } = new();
}
