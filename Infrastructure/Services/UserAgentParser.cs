using Application.Abstractions.Services;
using Domain.Enums;

namespace Infrastructure.Services;

/// <summary>
/// User-Agent parser servisi
/// Basit regex tabanlı - production'da UAParser.NET veya DeviceDetector.NET kullanılabilir
/// </summary>
public sealed class UserAgentParser : IUserAgentParser
{
    private static readonly string[] BotKeywords = 
    {
        "bot", "crawler", "spider", "slurp", "googlebot", "bingbot", 
        "yandex", "baidu", "duckduck", "facebot", "ia_archiver"
    };

    private static readonly string[] MobileKeywords =
    {
        "mobile", "android", "iphone", "ipod", "blackberry", "windows phone"
    };

    private static readonly string[] TabletKeywords =
    {
        "tablet", "ipad", "kindle", "playbook"
    };

    public DeviceType ParseDeviceType(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return DeviceType.Unknown;

        var ua = userAgent.ToLowerInvariant();

        if (IsBot(ua))
            return DeviceType.Bot;

        if (TabletKeywords.Any(k => ua.Contains(k)))
            return DeviceType.Tablet;

        if (MobileKeywords.Any(k => ua.Contains(k)))
            return DeviceType.Mobile;

        return DeviceType.Desktop;
    }

    public bool IsBot(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return false;

        var ua = userAgent.ToLowerInvariant();
        return BotKeywords.Any(k => ua.Contains(k));
    }
}
