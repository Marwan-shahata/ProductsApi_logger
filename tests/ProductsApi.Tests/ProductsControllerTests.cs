using Microsoft.AspNetCore.Mvc;
using ProductsApi.Api.Contracts;
using ProductsApi.Api.Controllers;
using ProductsApi.Application.Products.Dtos;
using ProductsApi.Tests.Support;

namespace ProductsApi.Tests;

public class ProductsControllerTests
{
    private static (ProductsController Controller, InMemoryProductStore Store) Create()
    {
        var (sender, store) = TestHost.Create();
        return (new ProductsController(sender), store);
    }

    [Fact]
    public async Task GetAll_Returns200WithProducts()
    {
        var (controller, store) = Create();
        store.Seed("A", 1m);

        var result = await controller.GetAll(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<ProductDto>>(ok.Value));
    }

    [Fact]
    public async Task GetById_WhenExists_Returns200()
    {
        var (controller, store) = Create();
        store.Seed("A", 1m);

        var result = await controller.GetById(1, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal("A", Assert.IsType<ProductDto>(ok.Value).Name);
    }

    [Fact]
    public async Task GetById_WhenMissing_Returns404()
    {
        var (controller, _) = Create();

        var result = await controller.GetById(99, CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFound.StatusCode);
    }

    [Fact]
    public async Task Create_Returns201WithLocationRoute()
    {
        var (controller, _) = Create();

        var result = await controller.Create(new CreateProductRequest("Mouse", 19.99m), CancellationToken.None);

        var created = Assert.IsType<CreatedAtRouteResult>(result.Result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal("GetProductById", created.RouteName);
        Assert.Equal(1, created.RouteValues!["id"]);
        Assert.Equal("Mouse", Assert.IsType<ProductDto>(created.Value).Name);
    }
}
