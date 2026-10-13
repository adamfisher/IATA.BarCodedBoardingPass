using System.Globalization;

namespace IATA.BarCodedBoardingPass.Internal;

/// <summary>
/// Walks the fixed-length fields of one section of a BCBP string, left to right.
/// </summary>
/// <remarks>
/// Reading past the end of a section is not an error: a field that starts beyond the end
/// is <see cref="FieldSpan.Missing"/> and a field that is cut short is returned as far as
/// it goes. This tolerates scanners and copy/paste stripping trailing space padding.
/// </remarks>
internal ref struct SectionCursor
{
    private readonly ReadOnlySpan<char> _barcode;
    private readonly int _end;
    private int _position;

    public SectionCursor(ReadOnlySpan<char> barcode, int start, int end)
    {
        _barcode = barcode;
        _position = start;
        _end = Math.Min(end, barcode.Length);
    }

    public readonly int Position => _position;

    public readonly bool IsAtEnd => _position >= _end;

    public readonly char? Peek() => IsAtEnd ? null : _barcode[_position];

    public FieldSpan Read(int length)
    {
        var field = IsAtEnd ? FieldSpan.Missing : new FieldSpan(_position, Math.Min(length, _end - _position));
        _position += length;
        return field;
    }

    public FieldSpan ReadToEnd() => IsAtEnd ? FieldSpan.Missing : Read(_end - _position);

    /// <summary>
    /// Reads a two character ASCII-printed hexadecimal "field size" item and returns a cursor
    /// over the section it describes. A missing or blank size counts as zero.
    /// </summary>
    public bool TryReadSection(string itemName, out SectionCursor section, out FieldSpan size, out ParseError error)
    {
        size = Read(2);
        var raw = size.Slice(_barcode);
        var length = 0;

        if (!raw.IsWhiteSpace() &&
            !int.TryParse(raw, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out length))
        {
            section = default;
            error = new ParseError($"{itemName} must be a two digit hexadecimal number but was \"{raw}\".", size.Start);
            return false;
        }

        section = new SectionCursor(_barcode, _position, _position + length);
        _position += length;
        error = default;
        return true;
    }
}

/// <summary>Why a barcode could not be parsed.</summary>
internal readonly record struct ParseError(string Message, int Position)
{
    public BarcodedBoardingPassFormatException ToException() => new(Message, Position);
}
