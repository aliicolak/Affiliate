using Application.Features.Conversions.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        // API key doğrulama
        var apiKey = Request.Headers["X-API-Key"].FirstOrDefault();
        var validApiKey = _configuration["Webhooks:ApiKey"];
        
        if (string.IsNullOrEmpty(apiKey) || apiKey != validApiKey)
        {
            return Unauthorized("Invalid API key");
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
}

public record ConversionPostbackDto(
    string TrackingCode,
    string OrderId,
    decimal Amount,
    string? Currency,
    string? ProductInfo,
    string? CustomerHash);
