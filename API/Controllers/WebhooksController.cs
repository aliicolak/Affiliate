using Application.Features.Conversions.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers;

/// <summary>
/// Merchant webhook endpoint'leri
/// </summary>
[ApiController]
[Route("api/webhooks")]
public sealed class WebhooksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public WebhooksController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    /// <summary>
    /// Merchant conversion postback endpoint
    /// </summary>
    [HttpPost("conversion")]
    [AllowAnonymous]
    public async Task<IActionResult> RecordConversion([FromBody] ConversionPostbackDto request)
    {
        // HMAC signature verification
        if (!VerifySignature())
        {
            return Unauthorized("Invalid signature");
        }

        try
        {
            var rawPostback = System.Text.Json.JsonSerializer.Serialize(request);
            
            var conversionId = await _mediator.Send(new RecordConversionCommand(
                request.TrackingCode,
                request.OrderId,
                request.Amount,
                request.Currency ?? "TRY",
                request.ProductInfo,
                request.CustomerHash,
                rawPostback));

            return Ok(new { conversionId, status = "recorded" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Pixel tracking endpoint (image pixel)
    /// </summary>
    [HttpGet("pixel")]
    [AllowAnonymous]
    public async Task<IActionResult> TrackPixel(
        [FromQuery] string code,
        [FromQuery] string orderId,
        [FromQuery] decimal amount,
        [FromQuery] string? currency)
    {
        try
        {
            await _mediator.Send(new RecordConversionCommand(
                code,
                orderId,
                amount,
                currency ?? "TRY",
                null, null, null));

            // 1x1 transparent GIF döndür
            var gif = Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7");
            return File(gif, "image/gif");
        }
        catch
        {
            var gif = Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7");
            return File(gif, "image/gif");
        }
    }

    /// <summary>
    /// HMAC-SHA256 signature verification
    /// </summary>
    private bool VerifySignature()
    {
        var apiKey = Request.Headers["X-API-Key"].FirstOrDefault();
        var signature = Request.Headers["X-Signature"].FirstOrDefault();
        var timestamp = Request.Headers["X-Timestamp"].FirstOrDefault();

        var validApiKey = _configuration["Webhooks:ApiKey"];
        var hmacSecret = _configuration["Webhooks:HmacSecret"];

        // API key check
        if (string.IsNullOrEmpty(apiKey) || apiKey != validApiKey)
        {
            return false;
        }

        // If no HMAC secret configured, allow API key only (backward compatible)
        if (string.IsNullOrEmpty(hmacSecret) || hmacSecret == "your-hmac-secret-min-32-chars-here")
        {
            return true;
        }

        // HMAC signature verification
        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(timestamp))
        {
            return false;
        }

        // Check timestamp (within 5 minutes to prevent replay attacks)
        if (long.TryParse(timestamp, out var ts))
        {
            var requestTime = DateTimeOffset.FromUnixTimeSeconds(ts);
            if (Math.Abs((DateTimeOffset.UtcNow - requestTime).TotalMinutes) > 5)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        // Compute expected signature
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(hmacSecret));
        var payload = $"{timestamp}.{apiKey}";
        var expectedSignature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));

        return signature == expectedSignature;
    }
}

public record ConversionPostbackDto(
    string TrackingCode,
    string OrderId,
    decimal Amount,
    string? Currency,
    string? ProductInfo,
    string? CustomerHash);
