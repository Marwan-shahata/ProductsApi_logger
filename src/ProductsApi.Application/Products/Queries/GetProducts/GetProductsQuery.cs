using MediatR;
using ProductsApi.Application.Products.Dtos;

namespace ProductsApi.Application.Products.Queries.GetProducts;

public sealed record GetProductsQuery : IRequest<IReadOnlyList<ProductDto>>;
