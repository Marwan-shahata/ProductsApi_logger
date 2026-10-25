using ProductsApi.Domain.Entities;

namespace ProductsApi.Tests;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties_AndTrimsName()
    {
        var product = Product.Create("  Keyboard  ", 49.99m);

        Assert.Equal("Keyboard", product.Name);
        Assert.Equal(49.99m, product.Price);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankName_Throws(string name) =>
        Assert.Throws<ArgumentException>(() => Product.Create(name, 1m));

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_WithNonPositivePrice_Throws(double price) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Product.Create("Mouse", (decimal)price));
}
