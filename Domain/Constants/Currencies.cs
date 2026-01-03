namespace Domain.Constants;

/// <summary>
/// Currency code constants (ISO 4217).
/// </summary>
public static class Currencies
{
    public const string TurkishLira = "TRY";
    public const string UsDollar = "USD";
    public const string Euro = "EUR";
    public const string BritishPound = "GBP";

    /// <summary>
    /// Default currency for the platform.
    /// </summary>
    public const string Default = TurkishLira;
}
