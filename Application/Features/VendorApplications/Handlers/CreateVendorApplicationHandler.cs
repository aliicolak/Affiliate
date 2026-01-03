using Application.Abstractions.Persistence;
using Application.Features.VendorApplications.Commands;
using Domain.Entities.Vendor;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.VendorApplications.Handlers;

/// <summary>
/// Satıcı başvurusu oluşturma handler'ı
/// </summary>
public sealed class CreateVendorApplicationHandler 
    : IRequestHandler<CreateVendorApplicationCommand, long>
{
    private readonly IAppDbContext _context;

    public CreateVendorApplicationHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(
        CreateVendorApplicationCommand request, 
        CancellationToken cancellationToken)
    {
        // Bekleyen başvuru kontrolü
        var hasPending = await _context.VendorApplications
            .AnyAsync(a => a.UserId == request.UserId && 
                          (a.Status == ApplicationStatus.Submitted || 
                           a.Status == ApplicationStatus.UnderReview), 
                      cancellationToken);

        if (hasPending)
            throw new InvalidOperationException("You already have a pending application");

        var application = new VendorApplication
        {
            UserId = request.UserId,
            CompanyName = request.CompanyName,
            ContactEmail = request.ContactEmail,
            Phone = request.Phone,
            WebsiteUrl = request.WebsiteUrl,
            Description = request.Description,
            CountryCode = request.CountryCode,
            TaxId = request.TaxId,
            Industry = request.Industry,
            EstimatedMonthlySales = request.EstimatedMonthlySales,
            ExistingAffiliateNetworks = request.ExistingAffiliateNetworks,
            Status = ApplicationStatus.Submitted
        };

        _context.VendorApplications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        return application.Id;
    }
}
