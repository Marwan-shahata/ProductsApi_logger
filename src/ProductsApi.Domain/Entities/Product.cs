namespace ProductsApi.Domain.Entities;

public class Product
{
    public const int NameMaxLength = 200;

    // Required by EF Core.
    private Product()
    {
        Name = string.Empty;
    }

    private Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    public static Product Create(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.", nameof(name));

        name = name.Trim();

        if (name.Length > NameMaxLength)
            throw new ArgumentException($"Product name must not exceed {NameMaxLength} characters.", nameof(name));

        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Product price must be greater than zero.");

        return new Product(name, price);
    }
}
