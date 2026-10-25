namespace ProductsApi.Api.Contracts;

/// <summary>Request body for creating a product.</summary>
public sealed record CreateProductRequest(string Name, decimal Price);
