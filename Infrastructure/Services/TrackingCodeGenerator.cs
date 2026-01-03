using Application.Abstractions.Services;
using System.Security.Cryptography;

namespace Infrastructure.Services;

/// <summary>
/// Benzersiz tracking kodu üretici
/// Base62 encoding kullanır (URL-safe)
/// </summary>
public sealed class TrackingCodeGenerator : ITrackingCodeGenerator
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int DefaultLength = 8;

    public string Generate() => Generate(DefaultLength);

    public string Generate(int length)
    {
        if (length < 4 || length > 16)
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be between 4 and 16");

        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = Chars[bytes[i] % Chars.Length];
        }

        return new string(result);
    }
}
