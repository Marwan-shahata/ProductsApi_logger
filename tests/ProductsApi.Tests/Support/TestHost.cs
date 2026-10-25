using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProductsApi.Application;
using ProductsApi.Application.Abstractions;

namespace ProductsApi.Tests.Support;

public static class TestHost
{
    /// <summary>Builds the real Application layer (MediatR + validators + pipeline) over a fake store.</summary>
    public static (ISender Sender, InMemoryProductStore Store) Create()
    {
        var store = new InMemoryProductStore();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddSingleton<IProductRepository>(store);
        services.AddSingleton<IUnitOfWork>(store);
        var provider = services.BuildServiceProvider();
        return (provider.GetRequiredService<ISender>(), store);
    }
}
