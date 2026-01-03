using Application.Features.Brands.Commands;
using Application.Features.Brands.Queries;
using Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages brand operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class BrandsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BrandsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all brands.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var result = await _mediator.Send(new GetAllBrandsQuery(includeInactive));
        return Ok(result);
    }

    /// <summary>
    /// Gets a brand by ID.
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _mediator.Send(new GetBrandByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new brand (Admin only).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateBrandCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Updates an existing brand (Admin only).
    /// </summary>
    [HttpPut("{id:long}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateBrandCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deletes a brand (Admin only).
    /// </summary>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _mediator.Send(new DeleteBrandCommand(id));
        return deleted ? NoContent() : NotFound();
    }
}

