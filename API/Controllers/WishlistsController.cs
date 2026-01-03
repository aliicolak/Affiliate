using Application.Features.Wishlists.Commands;
using Application.Features.Wishlists.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class WishlistsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WishlistsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetMyWishlists()
    {
        var result = await _mediator.Send(new GetUserWishlistsQuery(GetUserId()));
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, [FromQuery] string lang = "tr")
    {
        var result = await _mediator.Send(new GetWishlistByIdQuery(id, GetUserId(), lang));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWishlistRequest request)
    {
        var command = new CreateWishlistCommand(GetUserId(), request.Name, request.IsPublic);
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateWishlistRequest request)
    {
        var command = new UpdateWishlistCommand(id, GetUserId(), request.Name, request.IsPublic);
        var updated = await _mediator.Send(command);
        return updated ? Ok() : NotFound();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _mediator.Send(new DeleteWishlistCommand(id, GetUserId()));
        return deleted ? Ok() : NotFound();
    }

    [HttpPost("{id:long}/items")]
    public async Task<IActionResult> AddItem(long id, [FromBody] WishlistItemRequest request)
    {
        var result = await _mediator.Send(new AddItemToWishlistCommand(id, request.ProductId, GetUserId()));
        return result ? Ok() : BadRequest();
    }

    [HttpDelete("{id:long}/items/{productId:long}")]
    public async Task<IActionResult> RemoveItem(long id, long productId)
    {
        var result = await _mediator.Send(new RemoveItemFromWishlistCommand(id, productId, GetUserId()));
        return result ? Ok() : NotFound();
    }
}

public record CreateWishlistRequest(string Name, bool IsPublic = false);
public record UpdateWishlistRequest(string Name, bool IsPublic);
public record WishlistItemRequest(long ProductId);
