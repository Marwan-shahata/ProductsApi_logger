using ProductsApi.Application.Abstractions;
using ProductsApi.Domain.Entities;

namespace ProductsApi.Tests.Support;

/// <summary>
/// Plain in-memory fake of the repository + unit of work. It does NOT exercise EF Core or SQL Server.
/// </summary>
public sealed class InMemoryProductStore : IProductRepository, IUnitOfWork
{
    private readonly List<Product> _committed = [];
    private readonly List<Product> _pending = [];
    private int _nextId = 1;

    public int SaveCalls { get; private set; }

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Product>>(_committed.OrderBy(p => p.Id).ToList());

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        Task.FromResult(_committed.FirstOrDefault(p => p.Id == id));

    public Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        _pending.Add(product);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveCalls++;
        var count = _pending.Count;
        foreach (var product in _pending)
        {
            // Simulates the database-generated identity value.
            typeof(Product).GetProperty(nameof(Product.Id))!.SetValue(product, _nextId++);
            _committed.Add(product);
        }
        _pending.Clear();
        return Task.FromResult(count);
    }

    public void Seed(string name, decimal price)
    {
        _pending.Add(Product.Create(name, price));
        SaveChangesAsync(CancellationToken.None).GetAwaiter().GetResult();
        SaveCalls = 0;
    }
}
