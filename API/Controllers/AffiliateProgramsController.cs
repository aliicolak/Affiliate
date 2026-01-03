using Application.Features.AffiliatePrograms.Commands;
using Application.Features.AffiliatePrograms.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AffiliateProgramsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AffiliateProgramsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] long? merchantId = null)
    {
        var result = await _mediator.Send(new GetAllAffiliateProgramsQuery(merchantId));
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _mediator.Send(new GetAffiliateProgramByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateAffiliateProgramCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateAffiliateProgramCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        var updated = await _mediator.Send(command);
        return updated ? Ok() : NotFound();
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _mediator.Send(new DeleteAffiliateProgramCommand(id));
        return deleted ? Ok() : NotFound();
    }
}
