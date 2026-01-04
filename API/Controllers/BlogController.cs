using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.Features.Blog;
using Application.Features.Blog.Commands;
using Application.Features.Blog.Queries;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly IMediator _mediator;

    public BlogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ===== Posts =====

    /// <summary>
    /// Get paginated blog posts
    /// </summary>
    [HttpGet("posts")]
    public async Task<ActionResult<PaginatedBlogPostsResult>> GetPosts(
        [FromQuery] long? categoryId,
        [FromQuery] long? authorId,
        [FromQuery] string? tag,
        [FromQuery] string? search,
        [FromQuery] bool? isFeatured,
        [FromQuery] string sortBy = "newest",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetBlogPostsQuery(
            categoryId, authorId, tag, search, isFeatured, true, sortBy, page, pageSize
        ), ct);
        return Ok(result);
    }

    /// <summary>
    /// Get blog post by slug or id
    /// </summary>
    [HttpGet("posts/{slugOrId}")]
    public async Task<ActionResult<BlogPostDto>> GetPost(string slugOrId, CancellationToken ct)
    {
        BlogPostDto? result;
        if (long.TryParse(slugOrId, out var id))
        {
            result = await _mediator.Send(new GetBlogPostQuery(Id: id), ct);
        }
        else
        {
            result = await _mediator.Send(new GetBlogPostQuery(Slug: slugOrId), ct);
        }

        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Create a new blog post
    /// </summary>
    [HttpPost("posts")]
    [Authorize]
    public async Task<ActionResult<BlogPostDto>> CreatePost([FromBody] CreateBlogPostCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetPost), new { slugOrId = result.Slug }, result);
    }

    /// <summary>
    /// Update a blog post
    /// </summary>
    [HttpPut("posts/{id:long}")]
    [Authorize]
    public async Task<ActionResult<BlogPostDto>> UpdatePost(long id, [FromBody] UpdateBlogPostCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        var result = await _mediator.Send(command, ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Delete a blog post
    /// </summary>
    [HttpDelete("posts/{id:long}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(long id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteBlogPostCommand(id), ct);
        return result ? Ok() : NotFound();
    }

    /// <summary>
    /// Publish or unpublish a blog post
    /// </summary>
    [HttpPost("posts/{id:long}/publish")]
    [Authorize]
    public async Task<IActionResult> PublishPost(long id, [FromQuery] bool publish = true, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new PublishBlogPostCommand(id, publish), ct);
        return result ? Ok() : NotFound();
    }

    /// <summary>
    /// Get current user's blog posts
    /// </summary>
    [HttpGet("my-posts")]
    [Authorize]
    public async Task<ActionResult<PaginatedBlogPostsResult>> GetMyPosts(
        [FromQuery] bool includeDrafts = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        // userId will be injected by handler through ICurrentUserService
        var result = await _mediator.Send(new GetUserBlogPostsQuery(0, includeDrafts, page, pageSize), ct);
        return Ok(result);
    }

    // ===== Comments =====

    /// <summary>
    /// Get comments for a blog post
    /// </summary>
    [HttpGet("posts/{postId:long}/comments")]
    public async Task<ActionResult<PaginatedCommentsResult>> GetComments(
        long postId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetBlogCommentsQuery(postId, page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>
    /// Add a comment to a blog post
    /// </summary>
    [HttpPost("posts/{postId:long}/comments")]
    [Authorize]
    public async Task<ActionResult<BlogCommentDto>> AddComment(
        long postId,
        [FromBody] AddCommentRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateBlogCommentCommand(postId, request.Content, request.ParentCommentId), ct);
        return Ok(result);
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    [HttpDelete("comments/{id:long}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(long id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteBlogCommentCommand(id), ct);
        return result ? Ok() : NotFound();
    }

    // ===== Categories =====

    /// <summary>
    /// Get all blog categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<List<BlogCategoryDto>>> GetCategories(
        [FromQuery] bool includeEmpty = false,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetBlogCategoriesQuery(includeEmpty), ct);
        return Ok(result);
    }

    /// <summary>
    /// Create a blog category (Admin only)
    /// </summary>
    [HttpPost("categories")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BlogCategoryDto>> CreateCategory([FromBody] CreateBlogCategoryCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Update a blog category (Admin only)
    /// </summary>
    [HttpPut("categories/{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BlogCategoryDto>> UpdateCategory(long id, [FromBody] UpdateBlogCategoryCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        var result = await _mediator.Send(command, ct);
        return result is null ? NotFound() : Ok(result);
    }
}

// Request DTOs
public record AddCommentRequest(string Content, long? ParentCommentId = null);
