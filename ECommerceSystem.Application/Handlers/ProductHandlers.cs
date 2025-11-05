using AutoMapper;
using ECommerceSystem.Application.Commands;
using ECommerceSystem.Application.DTOs;
using ECommerceSystem.Application.Queries;
using ECommerceSystem.Application.Specifications;
using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Interfaces;
using MediatR;

namespace ECommerceSystem.Application.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = _mapper.Map<Product>(request.Product);
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return product.Id;
    }
}

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
        if (product == null)
            throw new KeyNotFoundException("Product not found");

        _mapper.Map(request.Product, product);
        product.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }
}

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
        if (product == null)
            throw new KeyNotFoundException("Product not found");

        await _unitOfWork.Products.DeleteAsync(request.Id);
        await _unitOfWork.SaveChangesAsync();
    }
}

public class GetProductsHandler : IRequestHandler<GetProductsQuery, PaginatedResponse<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var spec = new ProductSpecification(request);
        var (products, totalCount) = await _unitOfWork.Products.GetAsync(spec, request.PaginationParams);

        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

        return new PaginatedResponse<ProductDto>
        {
            Items = productDtos,
            TotalCount = totalCount,
            PageNumber = request.PaginationParams.PageNumber,
            PageSize = request.PaginationParams.PageSize
        };
    }
}

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }
}
