using MediatR;
using ProductsApi.Application.Products.Dtos;

namespace ProductsApi.Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(string Name, decimal Price) : IRequest<ProductDto>;
