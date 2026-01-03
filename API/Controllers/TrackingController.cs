using Application.Abstractions.Services;
using Application.Features.Tracking.Commands;
using Application.Features.Tracking.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

/// <summary>
/// Affiliate link tracking ve redirect controller
/// </summary>
[ApiController]
public sealed class TrackingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITrackingCodeGenerator _codeGenerator;

    public TrackingController(IMediator mediator, ITrackingCodeGenerator codeGenerator)
    {
        _mediator = mediator;
        _codeGenerator = codeGenerator;
    }

    /// <summary>
    /// Affiliate link redirect endpoint
    /// Public - authentication gerektirmez
    /// </summary>
    [HttpGet("/t/{code}")]
    [AllowAnonymous]
    public async Task<IActionResult> Redirect(
        string code,
        [FromQuery] long? o, // offer ID
        [FromQuery] long? p, // publisher ID
        [FromQuery] string? sub1,
        [FromQuery] string? sub2,
        [FromQuery] string? sub3)
    {
        if (!o.HasValue)
            return BadRequest("Offer ID is required");

        var ipAddress = GetClientIpAddress();
        var userAgent = Request.Headers.UserAgent.ToString();
        var referrer = Request.Headers.Referer.ToString();

        try
        {
            var result = await _mediator.Send(new RecordClickCommand(
                OfferId: o.Value,
                PublisherId: p,
                TrackingCode: code,
                IpAddress: ipAddress,
                UserAgent: userAgent,
                Referrer: referrer,
                SubId1: sub1,
                SubId2: sub2,
                SubId3: sub3
            ));

            return Redirect(result.RedirectUrl);
        }
        catch (InvalidOperationException)
        {
            return NotFound("Offer not found");
        }
    }

    /// <summary>
    /// Yeni tracking kodu üretir
    /// </summary>
    [HttpPost("api/tracking/generate-code")]
    [Authorize]
    public IActionResult GenerateCode()
    {
        var code = _codeGenerator.Generate();
        return Ok(new { code });
    }

    /// <summary>
    /// Tıklama istatistikleri
    /// </summary>
    [HttpGet("api/tracking/stats")]
    [Authorize]
    public async Task<IActionResult> GetStats(
        [FromQuery] long? publisherId,
        [FromQuery] long? merchantId,
        [FromQuery] long? offerId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? groupBy)
    {
        var result = await _mediator.Send(new GetClickStatsQuery(
            publisherId, merchantId, offerId, startDate, endDate, groupBy ?? "day"));
        
        return Ok(result);
    }

    /// <summary>
    /// Publisher'ın kendi tıklamaları
    /// </summary>
    [HttpGet("api/tracking/my-clicks")]
    [Authorize]
    public async Task<IActionResult> GetMyClicks(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new GetPublisherClicksQuery(userId, startDate, endDate, page, pageSize));
        return Ok(result);
    }

    private string GetClientIpAddress()
    {
        // Proxy/load balancer arkasında gerçek IP
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
    }
}
