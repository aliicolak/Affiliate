using Application.Features.Commissions.Commands;
using Application.Features.Commissions.Queries;
using Application.Features.Payouts.Commands;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

/// <summary>
/// Komisyon yönetimi controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CommissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Kendi komisyonlarım
    /// </summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyCommissions(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _mediator.Send(new GetPublisherCommissionsQuery(
            GetUserId(), startDate, endDate, status, page, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Kazanç özetim
    /// </summary>
    [HttpGet("my/summary")]
    public async Task<IActionResult> GetMySummary(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var result = await _mediator.Send(new GetPublisherEarningsSummaryQuery(
            GetUserId(), startDate, endDate));
        return Ok(result);
    }

    /// <summary>
    /// Ödeme talebi oluştur
    /// </summary>
    [HttpPost("payout-request")]
    public async Task<IActionResult> RequestPayout([FromBody] PayoutRequestDto request)
    {
        try
        {
            var id = await _mediator.Send(new CreatePayoutRequestCommand(
                GetUserId(),
                request.Amount,
                request.Currency ?? "TRY",
                request.PaymentMethod,
                request.PaymentDetails));
            return Ok(new { payoutId = id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Komisyon durumu güncelle (Admin)
    /// </summary>
    [HttpPut("{id:long}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(
        long id, 
        [FromBody] UpdateCommissionStatusDto request)
    {
        if (!Enum.TryParse<CommissionStatus>(request.Status, true, out var status))
            return BadRequest("Invalid status");

        var result = await _mediator.Send(new UpdateCommissionStatusCommand(
            id, status, request.Note, request.RejectionReason));
        
        return result ? Ok() : NotFound();
    }

    /// <summary>
    /// Payout işle (Admin)
    /// </summary>
    [HttpPut("payouts/{id:long}/process")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ProcessPayout(
        long id, 
        [FromBody] ProcessPayoutDto request)
    {
        if (!Enum.TryParse<PayoutStatus>(request.Status, true, out var status))
            return BadRequest("Invalid status");

        var result = await _mediator.Send(new ProcessPayoutCommand(
            id, status, request.TransactionRef, request.AdminNote, request.RejectionReason));
        
        return result ? Ok() : NotFound();
    }
}

public record PayoutRequestDto(
    decimal Amount,
    string? Currency,
    string PaymentMethod,
    string? PaymentDetails);

public record UpdateCommissionStatusDto(
    string Status,
    string? Note,
    string? RejectionReason);

public record ProcessPayoutDto(
    string Status,
    string? TransactionRef,
    string? AdminNote,
    string? RejectionReason);
