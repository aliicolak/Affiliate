using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.Features.Social;
using Application.Features.Social.Commands;
using Application.Features.Social.Queries;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SocialController : ControllerBase
{
    private readonly IMediator _mediator;

    public SocialController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ===== Shares =====

    /// <summary>
    /// Get feed of product shares
    /// </summary>
    [HttpGet("feed")]
    public async Task<ActionResult<PaginatedSharesResult>> GetFeed(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        // User ID is extracted from current user service in handler if logged in
        var result = await _mediator.Send(new GetFeedQuery(page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>
    /// Get filtered product shares
    /// </summary>
    [HttpGet("shares")]
    public async Task<ActionResult<PaginatedSharesResult>> GetShares(
        [FromQuery] long? userId,
        [FromQuery] long? offerId,
        [FromQuery] string? shareType,
        [FromQuery] string sortBy = "newest",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetSharesQuery(userId, offerId, shareType, sortBy, page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>
    /// Get product share by ID
    /// </summary>
    [HttpGet("shares/{id:long}")]
    public async Task<ActionResult<ProductShareDto>> GetShare(long id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetShareQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Create a product share
    /// </summary>
    [HttpPost("shares")]
    [Authorize]
    public async Task<ActionResult<ProductShareDto>> CreateShare([FromBody] CreateShareCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetShare), new { id = result.Id }, result);
    }

    /// <summary>
    /// Delete a product share
    /// </summary>
    [HttpDelete("shares/{id:long}")]
    [Authorize]
    public async Task<IActionResult> DeleteShare(long id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteShareCommand(id), ct);
        return result ? Ok() : NotFound();
    }

    // ===== Comments =====

    /// <summary>
    /// Get comments for a share
    /// </summary>
    [HttpGet("shares/{shareId:long}/comments")]
    public async Task<ActionResult<List<ShareCommentDto>>> GetComments(
        long shareId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetShareCommentsQuery(shareId, page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>
    /// Add a comment to a share
    /// </summary>
    [HttpPost("shares/{shareId:long}/comments")]
    [Authorize]
    public async Task<ActionResult<ShareCommentDto>> AddComment(
        long shareId,
        [FromBody] AddShareCommentRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateShareCommentCommand(shareId, request.Content, request.ParentCommentId), ct);
        return Ok(result);
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    [HttpDelete("comments/{id:long}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(long id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteShareCommentCommand(id), ct);
        return result ? Ok() : NotFound();
    }

    // ===== Likes =====

    /// <summary>
    /// Toggle like on an entity
    /// </summary>
    [HttpPost("like")]
    [Authorize]
    public async Task<ActionResult<LikeResult>> ToggleLike([FromBody] ToggleLikeCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    // ===== Follow =====

    /// <summary>
    /// Toggle follow a user
    /// </summary>
    [HttpPost("users/{userId:long}/follow")]
    [Authorize]
    public async Task<ActionResult<FollowResult>> ToggleFollow(long userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ToggleFollowCommand(userId), ct);
        return Ok(result);
    }

    /// <summary>
    /// Get user followers
    /// </summary>
    [HttpGet("users/{userId:long}/followers")]
    public async Task<ActionResult<PaginatedFollowersResult>> GetFollowers(
        long userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetFollowersQuery(userId, page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>
    /// Get users following
    /// </summary>
    [HttpGet("users/{userId:long}/following")]
    public async Task<ActionResult<PaginatedFollowersResult>> GetFollowing(
        long userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetFollowingQuery(userId, page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>
    /// Get user profile stats
    /// </summary>
    [HttpGet("users/{userId:long}/profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile(long userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserProfileQuery(userId), ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result ? Ok() : BadRequest();
    }
}

// Request DTOs
public record AddShareCommentRequest(string Content, long? ParentCommentId = null);
