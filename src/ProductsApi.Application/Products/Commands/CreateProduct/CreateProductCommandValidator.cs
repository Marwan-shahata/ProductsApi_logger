using FluentValidation;
using ProductsApi.Domain.Entities;

namespace ProductsApi.Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(Product.NameMaxLength)
            .WithMessage($"Name must not exceed {Product.NameMaxLength} characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.")
            .PrecisionScale(18, 2, ignoreTrailingZeros: false)
            .WithMessage("Price must have at most 16 digits before and 2 digits after the decimal point.");
    }
}
