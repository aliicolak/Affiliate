using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Domain.Services;

/// <summary>
/// URL-safe slug generator with Turkish character support.
/// </summary>
public static partial class SlugGenerator
{
    private static readonly Dictionary<char, string> TurkishCharMap = new()
    {
        { 'ç', "c" }, { 'Ç', "c" },
        { 'ğ', "g" }, { 'Ğ', "g" },
        { 'ı', "i" }, { 'İ', "i" },
        { 'ö', "o" }, { 'Ö', "o" },
        { 'ş', "s" }, { 'Ş', "s" },
        { 'ü', "u" }, { 'Ü', "u" }
    };

    /// <summary>
    /// Generates a URL-safe slug from the given text.
    /// </summary>
    /// <param name="text">The text to convert to a slug.</param>
    /// <param name="maxLength">Maximum length of the slug (default: 100).</param>
    /// <returns>A URL-safe slug.</returns>
    public static string Generate(string? text, int maxLength = 100)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Convert Turkish characters
        var sb = new StringBuilder(text.Length);
        foreach (var c in text)
        {
            sb.Append(TurkishCharMap.TryGetValue(c, out var replacement) ? replacement : c);
        }

        var normalized = sb.ToString()
            .Normalize(NormalizationForm.FormD);

        // Remove diacritics
        sb.Clear();
        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        var result = sb.ToString()
            .Normalize(NormalizationForm.FormC)
            .ToLowerInvariant();

        // Replace spaces and special characters with hyphens
        result = InvalidCharsRegex().Replace(result, "-");

        // Remove consecutive hyphens
        result = ConsecutiveHyphensRegex().Replace(result, "-");

        // Trim hyphens from start and end
        result = result.Trim('-');

        // Enforce max length
        if (result.Length > maxLength)
        {
            result = result[..maxLength].TrimEnd('-');
        }

        return result;
    }

    [GeneratedRegex(@"[^a-z0-9\-]")]
    private static partial Regex InvalidCharsRegex();

    [GeneratedRegex(@"-+")]
    private static partial Regex ConsecutiveHyphensRegex();
}
