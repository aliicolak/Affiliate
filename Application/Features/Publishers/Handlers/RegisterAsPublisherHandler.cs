using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Features.Publishers.Commands;
using Domain.Entities.Publisher;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Publishers.Handlers;

/// <summary>
/// Publisher kaydı handler'ı
/// </summary>
public sealed class RegisterAsPublisherHandler : IRequestHandler<RegisterAsPublisherCommand, long>
{
    private readonly IAppDbContext _context;
    private readonly ITrackingCodeGenerator _codeGenerator;

    public RegisterAsPublisherHandler(IAppDbContext context, ITrackingCodeGenerator codeGenerator)
    {
        _context = context;
        _codeGenerator = codeGenerator;
    }

    public async Task<long> Handle(RegisterAsPublisherCommand request, CancellationToken cancellationToken)
    {
        // Zaten publisher mı kontrol et
        var exists = await _context.Publishers
            .AnyAsync(p => p.UserId == request.UserId, cancellationToken);

        if (exists)
            throw new InvalidOperationException("User is already registered as a publisher");

        var publisher = new Publisher
        {
            UserId = request.UserId,
            Status = PublisherStatus.Active, // Otomatik onay
            PublisherCode = _codeGenerator.Generate(6),
            CompanyName = request.CompanyName,
            WebsiteUrl = request.WebsiteUrl,
            PromotionMethods = request.PromotionMethods,
            CountryCode = request.CountryCode,
            ApprovedUtc = DateTime.UtcNow
        };

        _context.Publishers.Add(publisher);
        await _context.SaveChangesAsync(cancellationToken);

        return publisher.Id;
    }
}
