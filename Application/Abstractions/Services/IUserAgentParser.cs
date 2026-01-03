using Domain.Enums;

namespace Application.Abstractions.Services;

/// <summary>
/// User-Agent parser servisi
/// </summary>
public interface IUserAgentParser
{
    /// <summary>
    /// User-Agent string'inden cihaz türünü belirler
    /// </summary>
    DeviceType ParseDeviceType(string? userAgent);
    
    /// <summary>
    /// Bot olup olmadığını kontrol eder
    /// </summary>
    bool IsBot(string? userAgent);
}
