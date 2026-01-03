using Application.Abstractions.Persistence;
using Application.Features.VendorApplications.Commands;
using Domain.Entities.Affiliate;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.VendorApplications.Handlers;

/// <summary>
/// Başvuru inceleme handler'ı (Admin)
/// Onay durumunda Merchant oluşturur
/// </summary>
public sealed class ReviewVendorApplicationHandler 
    : IRequestHandler<ReviewVendorApplicationCommand, long?>
{
    private readonly IAppDbContext _context;

    public ReviewVendorApplicationHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<long?> Handle(
        ReviewVendorApplicationCommand request, 
        CancellationToken cancellationToken)
    {
        var application = await _context.VendorApplications
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, cancellationToken);

        if (application is null)
            throw new InvalidOperationException("Application not found");

        application.Status = request.NewStatus;
        application.AdminNote = request.AdminNote;
        application.ReviewedUtc = DateTime.UtcNow;
        application.ReviewedByUserId = request.ReviewerId;

        long? merchantId = null;

        if (request.NewStatus == ApplicationStatus.Rejected)
        {
            application.RejectionReason = request.RejectionReason;
        }
        else if (request.NewStatus == ApplicationStatus.Approved)
        {
            // Merchant oluştur
            var merchant = new Merchant
            {
                Name = application.CompanyName,
                WebsiteUrl = application.WebsiteUrl ?? "",
                Description = application.Description,
                IsActive = true
            };

            _context.Merchants.Add(merchant);
            await _context.SaveChangesAsync(cancellationToken);

            application.CreatedMerchantId = merchant.Id;
            merchantId = merchant.Id;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return merchantId;
    }
}
