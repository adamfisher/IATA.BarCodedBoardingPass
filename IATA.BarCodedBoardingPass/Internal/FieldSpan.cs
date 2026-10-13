namespace IATA.BarCodedBoardingPass.Internal;

/// <summary>
/// Where a field sits in the barcode. <see cref="Length"/> can be shorter than the field's
/// defined size when the barcode ends early (trailing space padding is often stripped).
/// </summary>
internal readonly record struct FieldSpan(int Start, int Length)
{
    /// <summary>A field that is not in the barcode at all.</summary>
    public static FieldSpan Missing { get; } = new(-1, 0);

    public bool IsPresent => Start >= 0;

    public ReadOnlySpan<char> Slice(ReadOnlySpan<char> barcode) =>
        IsPresent ? barcode.Slice(Start, Length) : default;

    /// <summary>The field exactly as encoded, padding included, or <see langword="null"/> if missing.</summary>
    public string? ToRawString(ReadOnlySpan<char> barcode) =>
        IsPresent ? new string(Slice(barcode)) : null;

    /// <summary>The field without trailing padding, or <see langword="null"/> if missing or blank.</summary>
    public string? ToTrimmedString(ReadOnlySpan<char> barcode)
    {
        var value = Slice(barcode).TrimEnd(' ');
        return value.IsEmpty ? null : new string(value);
    }

    /// <summary>The single character code, or <see langword="null"/> if missing or blank.</summary>
    public char? ToChar(ReadOnlySpan<char> barcode) =>
        IsPresent && Length > 0 && barcode[Start] != ' ' ? barcode[Start] : null;
}
