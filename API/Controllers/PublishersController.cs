using Application.Features.Publishers.Commands;
using Application.Features.Publishers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

/// <summary>
/// Publisher (Affiliate yayıncı) controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PublishersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublishersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Publisher olarak kayıt ol
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterPublisherDto request)
    {
        try
        {
            var id = await _mediator.Send(new RegisterAsPublisherCommand(
                GetUserId(),
                request.CompanyName,
                request.WebsiteUrl,
                request.PromotionMethods,
                request.CountryCode ?? "TR"));
            
            return Ok(new { publisherId = id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Dashboard verileri
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var result = await _mediator.Send(new GetPublisherDashboardQuery(GetUserId()));
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Profil güncelle
    /// </summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdatePublisherProfileDto request)
    {
        // Önce publisher ID'yi bul
        var dashboard = await _mediator.Send(new GetPublisherDashboardQuery(GetUserId()));
        
        var result = await _mediator.Send(new UpdatePublisherProfileCommand(
            dashboard.Profile.Id,
            request.CompanyName,
            request.WebsiteUrl,
            request.PromotionMethods,
            request.TaxId,
            request.PreferredPaymentMethod ?? "Bank",
            request.PaymentDetails));

        return result ? Ok() : NotFound();
    }
}

public record RegisterPublisherDto(
    string? CompanyName,
    string? WebsiteUrl,
    string? PromotionMethods,
    string? CountryCode);

public record UpdatePublisherProfileDto(
    string? CompanyName,
    string? WebsiteUrl,
    string? PromotionMethods,
    string? TaxId,
    string? PreferredPaymentMethod,
    string? PaymentDetails);
