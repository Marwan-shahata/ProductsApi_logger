using FluentValidation;
using ProductsApi.Application.Products.Commands.CreateProduct;
using ProductsApi.Application.Products.Queries.GetProductById;
using ProductsApi.Application.Products.Queries.GetProducts;
using ProductsApi.Tests.Support;

namespace ProductsApi.Tests;

public class HandlerTests
{
    [Fact]
    public async Task CreateProduct_PersistsAndReturnsDtoWithGeneratedId()
    {
        var (sender, store) = TestHost.Create();

        var dto = await sender.Send(new CreateProductCommand("Keyboard", 49.99m));

        Assert.Equal(1, dto.Id);
        Assert.Equal("Keyboard", dto.Name);
        Assert.Equal(49.99m, dto.Price);
        Assert.Equal(1, store.SaveCalls);
    }

    [Fact]
    public async Task CreateProduct_InvalidCommand_ThrowsValidationException_AndDoesNotSave()
    {
        var (sender, store) = TestHost.Create();

        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => sender.Send(new CreateProductCommand("", -1m)));

        Assert.NotEmpty(ex.Errors);
        Assert.Equal(0, store.SaveCalls);
    }

    [Fact]
    public async Task GetProducts_ReturnsAllProducts()
    {
        var (sender, store) = TestHost.Create();
        store.Seed("A", 1m);
        store.Seed("B", 2m);

        var result = await sender.Send(new GetProductsQuery());

        Assert.Equal(["A", "B"], result.Select(p => p.Name));
    }

    [Fact]
    public async Task GetProductById_WhenExists_ReturnsDto()
    {
        var (sender, store) = TestHost.Create();
        store.Seed("A", 1m);

        var dto = await sender.Send(new GetProductByIdQuery(1));

        Assert.NotNull(dto);
        Assert.Equal("A", dto!.Name);
    }

    [Fact]
    public async Task GetProductById_WhenMissing_ReturnsNull()
    {
        var (sender, _) = TestHost.Create();
        Assert.Null(await sender.Send(new GetProductByIdQuery(42)));
    }

    [Fact]
    public async Task GetProductById_WithNonPositiveId_ThrowsValidationException()
    {
        var (sender, _) = TestHost.Create();
        await Assert.ThrowsAsync<ValidationException>(() => sender.Send(new GetProductByIdQuery(0)));
    }
}
