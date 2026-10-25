using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.Api.Contracts;
using ProductsApi.Application.Products.Commands.CreateProduct;
using ProductsApi.Application.Products.Dtos;
using ProductsApi.Application.Products.Queries.GetProductById;
using ProductsApi.Application.Products.Queries.GetProducts;

namespace ProductsApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController(ISender sender) : ControllerBase
{
    /// <summary>Returns all products.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await sender.Send(new GetProductsQuery(), cancellationToken);
        return Ok(products);
    }

    /// <summary>Returns a single product by id.</summary>
    [HttpGet("{id:int}", Name = "GetProductById")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await sender.Send(new GetProductByIdQuery(id), cancellationToken);

        return product is null
            ? NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Product not found",
                Detail = $"No product with id {id} exists."
            })
            : Ok(product);
    }

    /// <summary>Creates a new product.</summary>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var created = await sender.Send(new CreateProductCommand(request.Name, request.Price), cancellationToken);
        return CreatedAtRoute("GetProductById", new { id = created.Id }, created);
    }
}
