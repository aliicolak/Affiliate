namespace Application.Abstractions.Services;

/// <summary>
/// IP adresinden coğrafi konum çözümleyen servis
/// Open/Closed: Farklı provider'lar ile implement edilebilir
/// </summary>
public interface IGeoIpService
{
    /// <summary>
    /// IP adresinden coğrafi konum bilgisi döner
    /// </summary>
    Task<GeoIpResult> LookupAsync(string ipAddress, CancellationToken cancellationToken = default);
}

/// <summary>
/// GeoIP sorgu sonucu
/// </summary>
public sealed record GeoIpResult(
    string? CountryCode,
    string? CountryName,
    string? City,
    string? Region,
    double? Latitude,
    double? Longitude,
    string? Timezone
)
{
    public static GeoIpResult Empty => new(null, null, null, null, null, null, null);
}
