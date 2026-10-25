using MediatR;
using Microsoft.Extensions.Logging;
using ProductsApi.Application.Abstractions;
using ProductsApi.Application.Products.Dtos;
using ProductsApi.Domain.Entities;

namespace ProductsApi.Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<CreateProductCommandHandler> logger)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Price);

        await repository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Product created with Id {ProductId}", product.Id);

        return ProductDto.FromEntity(product);
    }
}
