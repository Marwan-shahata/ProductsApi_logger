using MediatR;
using ProductsApi.Application.Abstractions;
using ProductsApi.Application.Products.Dtos;

namespace ProductsApi.Application.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(IProductRepository repository)
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken);
        return product is null ? null : ProductDto.FromEntity(product);
    }
}
