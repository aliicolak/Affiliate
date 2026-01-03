using Application.Abstractions.Persistence;
using Application.Features.Publishers.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Publishers.Handlers;

/// <summary>
/// Publisher profil güncelleme handler'ı
/// </summary>
public sealed class UpdatePublisherProfileHandler : IRequestHandler<UpdatePublisherProfileCommand, bool>
{
    private readonly IAppDbContext _context;

    public UpdatePublisherProfileHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdatePublisherProfileCommand request, CancellationToken cancellationToken)
    {
        var publisher = await _context.Publishers
            .FirstOrDefaultAsync(p => p.Id == request.PublisherId, cancellationToken);

        if (publisher is null) return false;

        publisher.CompanyName = request.CompanyName;
        publisher.WebsiteUrl = request.WebsiteUrl;
        publisher.PromotionMethods = request.PromotionMethods;
        publisher.TaxId = request.TaxId;
        publisher.PreferredPaymentMethod = request.PreferredPaymentMethod;
        publisher.PaymentDetails = request.PaymentDetails;
        publisher.UpdatedUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
