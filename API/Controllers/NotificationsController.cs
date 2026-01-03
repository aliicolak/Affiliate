using Application.Abstractions.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers;

/// <summary>
/// Bildirim controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly IAppDbContext _context;

    public NotificationsController(IAppDbContext context)
    {
        _context = context;
    }

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Bildirimlerimi getir
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications(
        [FromQuery] bool? unreadOnly,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == GetUserId() && n.DeletedUtc == null);

        if (unreadOnly == true)
            query = query.Where(n => !n.IsRead);

        var totalCount = await query.CountAsync();
        var unreadCount = await query.CountAsync(n => !n.IsRead);

        var notifications = await query
            .OrderByDescending(n => n.CreatedUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new
            {
                n.Id,
                Type = n.Type.ToString(),
                n.Title,
                n.Message,
                n.IsRead,
                n.ActionUrl,
                n.CreatedUtc
            })
            .ToListAsync();

        return Ok(new
        {
            items = notifications,
            totalCount,
            unreadCount,
            page,
            pageSize
        });
    }

    /// <summary>
    /// Okundu olarak işaretle
    /// </summary>
    [HttpPut("{id:long}/read")]
    public async Task<IActionResult> MarkAsRead(long id)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == GetUserId());

        if (notification is null) return NotFound();

        notification.IsRead = true;
        notification.ReadUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok();
    }

    /// <summary>
    /// Tümünü okundu işaretle
    /// </summary>
    [HttpPut("mark-all-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == GetUserId() && !n.IsRead)
            .ToListAsync();

        foreach (var n in notifications)
        {
            n.IsRead = true;
            n.ReadUtc = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Ok(new { markedCount = notifications.Count });
    }
}
