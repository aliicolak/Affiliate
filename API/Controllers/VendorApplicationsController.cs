using Application.Features.VendorApplications.Commands;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

/// <summary>
/// Satıcı başvuru controller'ı
/// </summary>
[ApiController]
[Route("api/vendor-applications")]
[Authorize]
public sealed class VendorApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public VendorApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Satıcı olarak başvur
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Apply([FromBody] VendorApplicationDto request)
    {
        try
        {
            var id = await _mediator.Send(new CreateVendorApplicationCommand(
                GetUserId(),
                request.CompanyName,
                request.ContactEmail,
                request.Phone,
                request.WebsiteUrl,
                request.Description,
                request.CountryCode ?? "TR",
                request.TaxId,
                request.Industry,
                request.EstimatedMonthlySales,
                request.ExistingAffiliateNetworks));

            return Ok(new { applicationId = id, status = "Submitted" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Başvuru incele (Admin)
    /// </summary>
    [HttpPut("{id:long}/review")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Review(long id, [FromBody] ReviewApplicationDto request)
    {
        if (!Enum.TryParse<ApplicationStatus>(request.Status, true, out var status))
            return BadRequest("Invalid status");

        try
        {
            var merchantId = await _mediator.Send(new ReviewVendorApplicationCommand(
                id,
                status,
                GetUserId(),
                request.AdminNote,
                request.RejectionReason));

            return Ok(new { 
                status = status.ToString(), 
                merchantId 
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

public record VendorApplicationDto(
    string CompanyName,
    string ContactEmail,
    string? Phone,
    string? WebsiteUrl,
    string? Description,
    string? CountryCode,
    string? TaxId,
    string? Industry,
    string? EstimatedMonthlySales,
    string? ExistingAffiliateNetworks);

public record ReviewApplicationDto(
    string Status,
    string? AdminNote,
    string? RejectionReason);
