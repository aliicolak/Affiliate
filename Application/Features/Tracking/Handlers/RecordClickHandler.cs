using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Features.Tracking.Commands;
using Domain.Entities.Tracking;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tracking.Handlers;

/// <summary>
/// Tıklama kayıt handler'ı
/// Single Responsibility: Sadece tıklama kaydı yapar
/// </summary>
public sealed class RecordClickHandler : IRequestHandler<RecordClickCommand, RecordClickResult>
{
    private readonly IAppDbContext _context;
    private readonly IGeoIpService _geoIpService;
    private readonly IUserAgentParser _userAgentParser;

    public RecordClickHandler(
        IAppDbContext context,
        IGeoIpService geoIpService,
        IUserAgentParser userAgentParser)
    {
        _context = context;
        _geoIpService = geoIpService;
        _userAgentParser = userAgentParser;
    }

    public async Task<RecordClickResult> Handle(RecordClickCommand request, CancellationToken cancellationToken)
    {
        // Offer'ı bul
        var offer = await _context.Offers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.OfferId, cancellationToken);

        if (offer is null)
            throw new InvalidOperationException($"Offer not found: {request.OfferId}");

        // Bot kontrolü
        if (_userAgentParser.IsBot(request.UserAgent))
        {
            // Bot'ları kaydetme, direkt redirect
            return new RecordClickResult(0, offer.AffiliateUrl, false);
        }

        // GeoIP lookup
        var geoResult = await _geoIpService.LookupAsync(request.IpAddress, cancellationToken);
        
        // Cihaz türü
        var deviceType = _userAgentParser.ParseDeviceType(request.UserAgent);

        // Click event oluştur
        var clickEvent = new ClickEvent
        {
            OfferId = request.OfferId,
            PublisherId = request.PublisherId,
            TrackingCode = request.TrackingCode,
            IpAddress = HashIpAddress(request.IpAddress), // GDPR compliance
            UserAgent = request.UserAgent,
            Referrer = request.Referrer,
            DeviceType = deviceType,
            CountryCode = geoResult.CountryCode,
            City = geoResult.City,
            SubId1 = request.SubId1,
            SubId2 = request.SubId2,
            SubId3 = request.SubId3,
            IsConverted = false
        };

        _context.ClickEvents.Add(clickEvent);
        await _context.SaveChangesAsync(cancellationToken);

        return new RecordClickResult(clickEvent.Id, offer.AffiliateUrl, true);
    }

    /// <summary>
    /// IP adresini hash'ler (GDPR uyumluluğu)
    /// </summary>
    private static string HashIpAddress(string ip)
    {
        // İlk 3 oktet'i sakla, son oktet'i maskele
        var parts = ip.Split('.');
        if (parts.Length == 4)
        {
            return $"{parts[0]}.{parts[1]}.{parts[2]}.0";
        }
        return ip;
    }
}
