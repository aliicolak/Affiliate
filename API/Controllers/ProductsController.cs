using Application.Features.Products.Commands;
using Application.Features.Products.DTOs;
using Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllProductsRequestDto queryDto)
    {
        var result = await _mediator.Send(new GetAllProductsQuery(queryDto));
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, [FromQuery] string lang = "tr")
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id, lang));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        var updated = await _mediator.Send(command);
        return updated ? Ok() : NotFound();
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _mediator.Send(new DeleteProductCommand(id));
        return deleted ? Ok() : NotFound();
    }
}
