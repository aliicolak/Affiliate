using Application.Features.Categories.Commands;
using Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string lang = "tr", [FromQuery] bool includeInactive = false)
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery(lang, includeInactive));
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, [FromQuery] string lang = "tr")
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id, lang));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        var updated = await _mediator.Send(command);
        return updated ? Ok() : NotFound();
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _mediator.Send(new DeleteCategoryCommand(id));
        return deleted ? Ok() : NotFound();
    }
}
