using Application.Abstractions.Services;

namespace Infrastructure.Services;

/// <summary>
/// GeoIP servisi - gerçek implementasyon için MaxMind veya IP-API kullanılabilir
/// Şimdilik placeholder olarak boş dönüyor
/// </summary>
public sealed class GeoIpService : IGeoIpService
{
    public Task<GeoIpResult> LookupAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        // TODO: MaxMind GeoIP2 veya IP-API entegrasyonu
        // Şimdilik boş döner - production'da gerçek servis kullanılmalı
        
        // Localhost kontrolü
        if (ipAddress == "127.0.0.1" || ipAddress == "::1" || ipAddress.StartsWith("192.168."))
        {
            return Task.FromResult(new GeoIpResult("TR", "Turkey", "Istanbul", "Istanbul", 41.0082, 28.9784, "Europe/Istanbul"));
        }

        return Task.FromResult(GeoIpResult.Empty);
    }
}
