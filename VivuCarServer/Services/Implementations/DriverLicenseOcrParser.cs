using System.Globalization;
using System.Text.RegularExpressions;
using Services.Models.Admin;

namespace Services.Implementations;

public static partial class DriverLicenseOcrParser
{
    public static DriverLicenseOcrResult Parse(string rawText, float confidence, DateTime processedAt)
    {
        var normalized = Normalize(rawText);
        return new DriverLicenseOcrResult(
            normalized,
            MatchValue(normalized, FullNameRegex()),
            MatchValue(normalized, LicenseNumberRegex()) ?? MatchValue(normalized, AnyLongNumberRegex()),
            MatchValue(normalized, DateOfBirthRegex()),
            NormalizeLicenseClass(MatchValue(normalized, LicenseClassRegex()) ?? MatchValue(normalized, LicenseClassFallbackRegex())),
            NormalizeExpiry(MatchValue(normalized, ExpiryRegex()) ?? MatchValue(normalized, ExpiryFallbackRegex())),
            MathF.Round(Math.Clamp(confidence, 0, 1) * 100, 1),
            processedAt);
    }

    private static string Normalize(string value)
        => string.Join('\n', value.Replace("\r", string.Empty)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(line => Regex.Replace(line, @"\s+", " ")));

    private static string? MatchValue(string value, Regex regex)
    {
        var match = regex.Match(value);
        if (!match.Success) return null;
        var result = match.Groups["value"].Value.Trim(' ', ':', '-', '.', '|');
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }

    private static string? NormalizeLicenseClass(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.ToUpperInvariant();
        return normalized == "AZ" ? "A2" : normalized;
    }

    private static string? NormalizeExpiry(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (Regex.IsMatch(value, @"(?i)kh[oô]ng\s+th[ờo]i\s+h[aạ]n"))
            return "Không thời hạn";
        return value;
    }

    [GeneratedRegex(@"(?im)(?:Họ\s*(?:và\s*)?tên|Ho\s*(?:va\s*)?ten|H[oọ]\s*t[eêé]n(?:\s*/\s*Fu[iIl]{2}\s*name)?|Full\s*name)(?:\s*/\s*(?:Full\s*name|Họ\s*(?:và\s*)?tên))?\s*[:.]?\s*(?<value>[^\n]{3,80})")]
    private static partial Regex FullNameRegex();

    [GeneratedRegex(@"(?im)(?:Số\s*/?\s*No|So\s*/?\s*No|Số\s*GPLX|License\s*(?:No|Number))\s*[:.]?\s*(?<value>[0-9]{8,15})")]
    private static partial Regex LicenseNumberRegex();

    [GeneratedRegex(@"(?m)\b(?<value>[0-9]{10,15})\b")]
    private static partial Regex AnyLongNumberRegex();

    [GeneratedRegex(@"(?im)(?:Ngày\s*sinh(?:\s*/\s*Date\s*of\s*Birth)?|Date\s*of\s*Birth|Sinh)\s*[:.]?\s*(?<value>[0-3]?\d[./-][01]?\d[./-](?:19|20)\d{2})")]
    private static partial Regex DateOfBirthRegex();

    [GeneratedRegex(@"(?im)(?:Hạng(?:\s*/\s*Class)?|Hang|Class)\s*[:.]?\s*(?<value>[A-Z][A-Z0-9]{0,3})\b")]
    private static partial Regex LicenseClassRegex();

    [GeneratedRegex(@"(?m)^\s*(?<value>A[12Z]|B[12]|C|D[12]?|E|F[A-F]?)\s*$")]
    private static partial Regex LicenseClassFallbackRegex();

    [GeneratedRegex(@"(?im)(?:Có\s*giá\s*trị\s*đến(?:\s*/\s*Expires)?|Date\s*of\s*Expiry|Expires?)\s*[:.]?\s*(?<value>Không\s*thời\s*hạn|[0-3]?\d[./-][01]?\d[./-](?:19|20)\d{2})")]
    private static partial Regex ExpiryRegex();

    [GeneratedRegex(@"(?im)(?<value>kh[oô]ng\s+th[ờo]i\s+h[aạ]n)")]
    private static partial Regex ExpiryFallbackRegex();
}