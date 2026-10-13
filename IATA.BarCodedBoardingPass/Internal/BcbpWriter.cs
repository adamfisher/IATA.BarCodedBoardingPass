using System.Globalization;

namespace IATA.BarCodedBoardingPass.Internal;

/// <summary>
/// Writes fixed-length BCBP fields into a span. Sections are prefixed with their size in
/// hexadecimal, and a section ends after its last field that has a value: trailing fields
/// without a value are left out rather than padded.
/// </summary>
/// <remarks>
/// Fields that do not fit in the destination are skipped but still counted, so section sizes
/// stay correct and trailing fields that are later dropped may briefly pass the end. Check
/// <see cref="Overflowed"/> once everything is written.
/// </remarks>
internal ref struct BcbpWriter(Span<char> destination)
{
    private readonly Span<char> _destination = destination;
    private int _position;
    private int _lastValueEnd;

    public readonly int Written => _position;

    public readonly bool Overflowed => _position > _destination.Length;

    public void Text(string? value, int length)
    {
        if (TryAdvance(length, out var field))
        {
            field.Fill(' ');
            value.AsSpan(0, Math.Min(value?.Length ?? 0, length)).CopyTo(field);
        }

        if (value is not null)
        {
            _lastValueEnd = _position;
        }
    }

    public void Text(string? value) => Text(value, value?.Length ?? 0);

    public void Char(char? value)
    {
        if (TryAdvance(1, out var field))
        {
            field[0] = value ?? ' ';
        }

        if (value is not null)
        {
            _lastValueEnd = _position;
        }
    }

    public void Date(DateOnly? value, bool withYear)
    {
        var length = withYear ? 4 : 3;

        if (value is not { } date)
        {
            Text(null, length);
            return;
        }

        if (TryAdvance(length, out var field))
        {
            if (withYear)
            {
                DayOfYear.FormatWithYear(date, field);
            }
            else
            {
                DayOfYear.Format(date, field);
            }
        }

        _lastValueEnd = _position;
    }

    public Section BeginSection()
    {
        var section = new Section(_position);
        TryAdvance(2, out _);
        _lastValueEnd = _position;
        return section;
    }

    public void EndSection(Section section)
    {
        var contentStart = section.SizeAt + 2;
        var length = _lastValueEnd - contentStart;

        if (length > 0xFF)
        {
            throw new InvalidOperationException(
                $"A section of {length} characters is too long to encode; the maximum is 255.");
        }

        _position = contentStart + length;
        _lastValueEnd = _position;

        if (section.SizeAt + 2 <= _destination.Length)
        {
            length.TryFormat(_destination.Slice(section.SizeAt, 2), out _, "X2", CultureInfo.InvariantCulture);
        }
    }

    private bool TryAdvance(int length, out Span<char> field)
    {
        var start = _position;
        _position += length;

        if (_position > _destination.Length)
        {
            field = default;
            return false;
        }

        field = _destination[start.._position];
        return true;
    }

    public readonly record struct Section(int SizeAt);
}
