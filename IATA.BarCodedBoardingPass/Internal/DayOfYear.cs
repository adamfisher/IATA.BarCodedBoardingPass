using System.Globalization;

namespace IATA.BarCodedBoardingPass.Internal;

/// <summary>
/// BCBP dates are a day of the year (<c>DDD</c>), optionally preceded by the last digit of the
/// year (<c>YDDD</c>). These helpers turn them into calendar dates.
/// </summary>
internal static class DayOfYear
{
    /// <summary>
    /// Date of issue (<c>YDDD</c>): the latest date with that year digit and day that is not
    /// after <paramref name="reference"/> (allowing a day for time zones).
    /// </summary>
    public static DateOnly? ParseIssueDate(ReadOnlySpan<char> value, DateOnly reference)
    {
        if (value.Length != 4 || !char.IsAsciiDigit(value[0]) || !TryParseDay(value[1..], out var day))
        {
            return null;
        }

        var latest = reference.AddDays(1);
        var year = latest.Year - ((latest.Year - (value[0] - '0')) % 10 + 10) % 10;

        for (; year > latest.Year - 20; year -= 10)
        {
            if (TryCreate(year, day, out var date) && date <= latest)
            {
                return date;
            }
        }

        return null;
    }

    /// <summary>
    /// Date of flight (<c>DDD</c>): the first such day on or after <paramref name="issueDate"/>
    /// when there is one, otherwise the such day nearest to <paramref name="reference"/>.
    /// </summary>
    public static DateOnly? ParseFlightDate(ReadOnlySpan<char> value, DateOnly? issueDate, DateOnly reference)
    {
        if (!TryParseDay(value, out var day))
        {
            return null;
        }

        if (issueDate is { } issued)
        {
            for (var year = issued.Year; year <= issued.Year + 4; year++)
            {
                if (TryCreate(year, day, out var date) && date >= issued)
                {
                    return date;
                }
            }

            return null;
        }

        DateOnly? nearest = null;

        for (var year = reference.Year - 1; year <= reference.Year + 1; year++)
        {
            if (TryCreate(year, day, out var date) &&
                (nearest is null || Distance(date, reference) < Distance(nearest.Value, reference)))
            {
                nearest = date;
            }
        }

        return nearest;
    }

    public static void Format(DateOnly date, Span<char> destination) =>
        date.DayOfYear.TryFormat(destination, out _, "D3", CultureInfo.InvariantCulture);

    public static void FormatWithYear(DateOnly date, Span<char> destination)
    {
        destination[0] = (char)('0' + date.Year % 10);
        Format(date, destination[1..]);
    }

    private static int Distance(DateOnly a, DateOnly b) => Math.Abs(a.DayNumber - b.DayNumber);

    private static bool TryParseDay(ReadOnlySpan<char> value, out int day)
    {
        day = 0;

        if (value.Length != 3)
        {
            return false;
        }

        foreach (var c in value)
        {
            if (!char.IsAsciiDigit(c))
            {
                return false;
            }

            day = day * 10 + (c - '0');
        }

        return day is >= 1 and <= 366;
    }

    private static bool TryCreate(int year, int day, out DateOnly date)
    {
        date = default;

        if (year is < 1 or > 9999 || day > (DateTime.IsLeapYear(year) ? 366 : 365))
        {
            return false;
        }

        date = new DateOnly(year, 1, 1).AddDays(day - 1);
        return true;
    }
}
