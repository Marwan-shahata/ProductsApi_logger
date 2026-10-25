using ProductsApi.Application.Products.Commands.CreateProduct;

namespace ProductsApi.Tests;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_Passes() =>
        Assert.True(_validator.Validate(new CreateProductCommand("Mouse", 19.99m)).IsValid);

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyName_Fails(string name)
    {
        var result = _validator.Validate(new CreateProductCommand(name, 10m));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProductCommand.Name));
    }

    [Fact]
    public void TooLongName_Fails()
    {
        var result = _validator.Validate(new CreateProductCommand(new string('a', 201), 10m));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProductCommand.Name));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NonPositivePrice_Fails(double price)
    {
        var result = _validator.Validate(new CreateProductCommand("Mouse", (decimal)price));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProductCommand.Price));
    }

    [Fact]
    public void PriceWithMoreThanTwoDecimals_Fails()
    {
        var result = _validator.Validate(new CreateProductCommand("Mouse", 1.999m));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProductCommand.Price));
    }
}
