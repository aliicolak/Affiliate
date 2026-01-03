namespace Domain.ValueObjects;

/// <summary>
/// Benzersiz tracking kodu - Immutable Value Object
/// </summary>
public sealed class TrackingCode : IEquatable<TrackingCode>
{
    public string Value { get; }

    private TrackingCode(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Yeni benzersiz tracking kodu oluşturur
    /// Format: 8 karakter Base62 (a-zA-Z0-9)
    /// </summary>
    public static TrackingCode Generate()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var code = new char[8];
        
        for (int i = 0; i < code.Length; i++)
        {
            code[i] = chars[random.Next(chars.Length)];
        }
        
        return new TrackingCode(new string(code));
    }

    /// <summary>
    /// Mevcut bir kodu Value Object'e dönüştürür
    /// </summary>
    public static TrackingCode From(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Tracking code cannot be empty", nameof(code));

        if (code.Length < 6 || code.Length > 12)
            throw new ArgumentException("Tracking code must be between 6-12 characters", nameof(code));

        return new TrackingCode(code);
    }

    public bool Equals(TrackingCode? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as TrackingCode);
    
    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static implicit operator string(TrackingCode code) => code.Value;
}
