using Application.Features.Social;
using Application.Features.Social.Commands;
using Application.Features.Social.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CollectionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CollectionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<CollectionDto>>> GetMyCollections()
    {
        return Ok(await _mediator.Send(new GetMyCollectionsQuery()));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<CollectionDetailDto>> GetCollection(long id)
    {
        return Ok(await _mediator.Send(new GetCollectionDetailQuery(id)));
    }

    [HttpGet("user/{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<CollectionDto>>> GetUserCollections(long userId)
    {
        return Ok(await _mediator.Send(new GetUserCollectionsQuery(userId)));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<List<CollectionDto>>> Search([FromQuery] string? q)
    {
        return Ok(await _mediator.Send(new SearchCollectionsQuery(q)));
    }

    [HttpPost]
    public async Task<ActionResult<long>> Create([FromBody] CreateCollectionCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        await _mediator.Send(new DeleteCollectionCommand(id));
        return NoContent();
    }

    [HttpPost("{collectionId}/items")]
    public async Task<ActionResult<long>> AddItem(long collectionId, [FromBody] AddItemRequest request)
    {
        return Ok(await _mediator.Send(new AddItemToCollectionCommand(collectionId, request.EntityType, request.EntityId)));
    }

    [HttpDelete("items/{itemId}")]
    public async Task<ActionResult> RemoveItem(long itemId)
    {
        await _mediator.Send(new RemoveItemFromCollectionCommand(itemId));
        return NoContent();
    }
}

public record AddItemRequest(string EntityType, long EntityId);
