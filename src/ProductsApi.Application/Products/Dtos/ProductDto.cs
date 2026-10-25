using ProductsApi.Domain.Entities;

namespace ProductsApi.Application.Products.Dtos;

public sealed record ProductDto(int Id, string Name, decimal Price)
{
    public static ProductDto FromEntity(Product product) =>
        new(product.Id, product.Name, product.Price);
}
