using MediatR;
using ProductsApi.Application.Abstractions;
using ProductsApi.Application.Products.Dtos;

namespace ProductsApi.Application.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler(IProductRepository repository)
    : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await repository.GetAllAsync(cancellationToken);
        return products.Select(ProductDto.FromEntity).ToList();
    }
}
