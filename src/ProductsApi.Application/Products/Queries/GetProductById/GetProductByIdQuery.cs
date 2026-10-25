using MediatR;
using ProductsApi.Application.Products.Dtos;

namespace ProductsApi.Application.Products.Queries.GetProductById;

/// <summary>Returns null when the product does not exist.</summary>
public sealed record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;
