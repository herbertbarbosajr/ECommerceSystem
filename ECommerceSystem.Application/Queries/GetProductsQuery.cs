using ECommerceSystem.Application.DTOs;
using ECommerceSystem.Core.Common;
using MediatR;

namespace ECommerceSystem.Application.Queries;

public class GetProductsQuery : IRequest<PaginatedResponse<ProductDto>>
{
    public bool OnlyActive { get; set; } = true;
    public string? Search { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; } = "Name";
    public bool SortDescending { get; set; } = false;
    public PaginationParams PaginationParams { get; set; } = new();
}
