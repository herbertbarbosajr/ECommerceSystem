using AutoMapper;
using ECommerceSystem.Application.Commands;
using ECommerceSystem.Application.DTOs;
using ECommerceSystem.Application.Queries;
using ECommerceSystem.Application.Specifications;
using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Interfaces;
using MediatR;

namespace ECommerceSystem.Application.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateOrderHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                UserId = request.UserId,
                PaymentMethod = request.Order.PaymentMethod,
                OrderItems = new List<OrderItem>()
            };

            decimal totalAmount = 0;

            foreach (var item in request.Order.OrderItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new KeyNotFoundException($"Product {item.ProductId} not found");

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}");

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                order.OrderItems.Add(orderItem);
                totalAmount += orderItem.TotalPrice;

                // Update stock
                product.StockQuantity -= item.Quantity;
                await _unitOfWork.Products.UpdateAsync(product);
            }

            order.TotalAmount = totalAmount;
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return order.Id;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}

public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderStatusHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
        if (order == null)
            throw new KeyNotFoundException("Order not found");

        order.Status = request.Status;
        if (request.Status == OrderStatus.Confirmed)
            order.IsPaid = true;

        await _unitOfWork.Orders.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
    }
}

public class GetOrdersByUserHandler : IRequestHandler<GetOrdersByUserQuery, PaginatedResponse<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrdersByUserHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<OrderDto>> Handle(GetOrdersByUserQuery request, CancellationToken cancellationToken)
    {
        var spec = new OrderSpecification(request);
        var (orders, totalCount) = await _unitOfWork.Orders.GetAsync(spec, request.PaginationParams);

        var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

        return new PaginatedResponse<OrderDto>
        {
            Items = orderDtos,
            TotalCount = totalCount,
            PageNumber = request.PaginationParams.PageNumber,
            PageSize = request.PaginationParams.PageSize
        };
    }
}

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.Id);
        return order == null ? null : _mapper.Map<OrderDto>(order);
    }
}
