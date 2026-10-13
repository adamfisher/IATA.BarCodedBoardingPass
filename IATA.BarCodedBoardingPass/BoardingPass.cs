using System.Diagnostics.CodeAnalysis;
using IATA.BarCodedBoardingPass.Internal;

namespace IATA.BarCodedBoardingPass;

/// <summary>
/// An IATA Bar Coded Boarding Pass (Resolution 792, versions 1 to 8). Text values are as
/// encoded, without trailing padding; a value that is blank or not encoded is <see langword="null"/>.
/// </summary>
/// <example>
/// <code>
/// var pass = BoardingPass.Parse(scannedText);
/// var leg = pass.Legs[0];
/// Console.WriteLine($"{pass.PassengerName}: {leg.OperatingCarrier}{leg.FlightNumber} {leg.FromAirport}-{leg.ToAirport} on {leg.FlightDate}");
///
/// var reissued = pass with { Legs = [leg with { SeatNumber = "014C" }] };
/// string barcode = reissued.Encode();
/// </code>
/// </example>
public sealed record BoardingPass : ISpanParsable<BoardingPass>
{
    /// <summary>Format Code, <c>M</c> for the standard multiple-leg format.</summary>
    public char FormatCode { get; init; } = 'M';

    /// <summary>Passenger Name as <c>LAST/FIRST</c>, up to 20 characters (longer names are truncated by the issuer).</summary>
    public string? PassengerName { get; init; }

    /// <summary>Electronic Ticket Indicator, <c>E</c> for an electronic ticket.</summary>
    public char? ElectronicTicketIndicator { get; init; } = 'E';

    /// <summary>The flights on this boarding pass, 1 to 9 (the standard allows up to 4).</summary>
    public IReadOnlyList<FlightLeg> Legs { get; init; } = [];

    /// <summary>Version Number of the standard the pass was encoded with; <see langword="null"/> when it has no conditional items.</summary>
    public int? Version { get; init; }

    /// <summary>Passenger Description.</summary>
    public PassengerDescription? PassengerDescription { get; init; }

    /// <summary>Source of check-in.</summary>
    public CheckInSource? CheckInSource { get; init; }

    /// <summary>Source of Boarding Pass Issuance.</summary>
    public BoardingPassIssuanceSource? IssuanceSource { get; init; }

    /// <summary>
    /// Date of Issue of Boarding Pass. The barcode holds the last digit of the year and the day
    /// of the year; this is the latest matching date that is not after the parse reference date.
    /// </summary>
    public DateOnly? IssueDate { get; init; }

    /// <summary>Document Type.</summary>
    public DocumentType? DocumentType { get; init; }

    /// <summary>Airline Designator of the boarding pass issuer, up to 3 characters.</summary>
    public string? IssuingAirline { get; init; }

    /// <summary>
    /// Baggage Tag License Plate Number(s), 13 characters: a 10 digit tag number followed by
    /// the number of consecutive tags (version 7 onwards; earlier versions count from 0).
    /// </summary>
    public string? BaggageTagNumber { get; init; }

    /// <summary>1st Non-Consecutive Baggage Tag License Plate Number(s).</summary>
    public string? FirstNonConsecutiveBaggageTagNumber { get; init; }

    /// <summary>2nd Non-Consecutive Baggage Tag License Plate Number(s).</summary>
    public string? SecondNonConsecutiveBaggageTagNumber { get; init; }

    /// <summary>Security data, such as a digital signature, when present.</summary>
    public SecurityData? Security { get; init; }

    /// <summary>
    /// Characters after the last leg and the security data that are not part of the BCBP
    /// structure. Usually a sign of a malformed barcode. Not written by <see cref="Encode"/>.
    /// </summary>
    public string? TrailingData { get; init; }

    /// <summary>The part of <see cref="PassengerName"/> before the <c>/</c>.</summary>
    public string? LastName => PassengerName?.IndexOf('/', StringComparison.Ordinal) is int slash and >= 0
        ? PassengerName[..slash]
        : PassengerName;

    /// <summary>The part of <see cref="PassengerName"/> after the <c>/</c>, or <see langword="null"/> if there is none.</summary>
    public string? FirstName => PassengerName?.IndexOf('/', StringComparison.Ordinal) is int slash and >= 0 && slash < PassengerName.Length - 1
        ? PassengerName[(slash + 1)..]
        : null;

    #region Parsing

    /// <summary>Parses a boarding pass.</summary>
    /// <param name="s">The text scanned from the barcode.</param>
    /// <param name="referenceDate">
    /// A date near the scan, used to work out the year of the issue and flight dates, which the
    /// barcode does not hold in full. Defaults to today (UTC).
    /// </param>
    /// <exception cref="BarcodedBoardingPassFormatException"><paramref name="s"/> is not a boarding pass.</exception>
    public static BoardingPass Parse(string s, DateOnly? referenceDate = null)
    {
        ArgumentNullException.ThrowIfNull(s);
        return Parse(s.AsSpan(), referenceDate);
    }

    /// <inheritdoc cref="Parse(string, DateOnly?)"/>
    public static BoardingPass Parse(ReadOnlySpan<char> s, DateOnly? referenceDate = null) =>
        TryParse(s, referenceDate, out var result, out var error) ? result : throw error.ToException();

    /// <summary>Tries to parse a boarding pass, using today (UTC) to work out the year of its dates.</summary>
    /// <param name="s">The text scanned from the barcode.</param>
    /// <param name="result">The boarding pass, when <paramref name="s"/> is one.</param>
    public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out BoardingPass result) =>
        TryParse(s.AsSpan(), null, out result, out _);

    /// <summary>Tries to parse a boarding pass.</summary>
    /// <param name="s">The text scanned from the barcode.</param>
    /// <param name="referenceDate">A date near the scan, used to work out the year of the issue and flight dates.</param>
    /// <param name="result">The boarding pass, when <paramref name="s"/> is one.</param>
    public static bool TryParse([NotNullWhen(true)] string? s, DateOnly? referenceDate, [MaybeNullWhen(false)] out BoardingPass result) =>
        TryParse(s.AsSpan(), referenceDate, out result, out _);

    /// <inheritdoc cref="TryParse(string?, DateOnly?, out BoardingPass)"/>
    public static bool TryParse(ReadOnlySpan<char> s, DateOnly? referenceDate, [MaybeNullWhen(false)] out BoardingPass result) =>
        TryParse(s, referenceDate, out result, out _);

    static BoardingPass IParsable<BoardingPass>.Parse(string s, IFormatProvider? provider) => Parse(s);

    static BoardingPass ISpanParsable<BoardingPass>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s);

    static bool IParsable<BoardingPass>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out BoardingPass result) =>
        TryParse(s, out result);

    static bool ISpanParsable<BoardingPass>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out BoardingPass result) =>
        TryParse(s, null, out result);

    private static bool TryParse(ReadOnlySpan<char> s, DateOnly? referenceDate, [MaybeNullWhen(false)] out BoardingPass result, out ParseError error)
    {
        if (!BcbpLayout.TryRead(s, out var layout, out error))
        {
            result = null;
            return false;
        }

        var reference = referenceDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var issueDate = DayOfYear.ParseIssueDate(layout.DateOfIssue.Slice(s), reference);
        var legs = new FlightLeg[layout.Legs.Count];

        for (var i = 0; i < legs.Length; i++)
        {
            var leg = layout.Legs[i];
            legs[i] = new FlightLeg
            {
                OperatingCarrierPnrCode = leg.OperatingCarrierPnrCode.ToTrimmedString(s),
                FromAirport = leg.FromCityAirportCode.ToTrimmedString(s),
                ToAirport = leg.ToCityAirportCode.ToTrimmedString(s),
                OperatingCarrier = leg.OperatingCarrierDesignator.ToTrimmedString(s),
                FlightNumber = leg.FlightNumber.ToTrimmedString(s),
                FlightDate = DayOfYear.ParseFlightDate(leg.DateOfFlight.Slice(s), issueDate, reference),
                CompartmentCode = leg.CompartmentCode.ToChar(s),
                SeatNumber = leg.SeatNumber.ToTrimmedString(s),
                CheckInSequenceNumber = leg.CheckInSequenceNumber.ToTrimmedString(s),
                PassengerStatus = leg.PassengerStatus.ToChar(s),
                AirlineNumericCode = leg.AirlineNumericCode.ToTrimmedString(s),
                DocumentFormSerialNumber = leg.DocumentFormSerialNumber.ToTrimmedString(s),
                SelecteeIndicator = leg.SelecteeIndicator.ToChar(s),
                InternationalDocumentationVerification = (InternationalDocumentationVerification?)leg.InternationalDocumentVerification.ToChar(s),
                MarketingCarrier = leg.MarketingCarrierDesignator.ToTrimmedString(s),
                FrequentFlyerAirline = leg.FrequentFlyerAirlineDesignator.ToTrimmedString(s),
                FrequentFlyerNumber = leg.FrequentFlyerNumber.ToTrimmedString(s),
                IdAdIndicator = leg.IdAdIndicator.ToChar(s),
                FreeBaggageAllowance = leg.FreeBaggageAllowance.ToTrimmedString(s),
                FastTrack = leg.FastTrack.ToChar(s) switch { 'Y' => true, 'N' => false, _ => null },
                AirlineIndividualUse = leg.AirlineIndividualUse.ToTrimmedString(s),
            };
        }

        result = new BoardingPass
        {
            FormatCode = s[0],
            PassengerName = layout.PassengerName.ToTrimmedString(s),
            ElectronicTicketIndicator = layout.ElectronicTicketIndicator.ToChar(s),
            Legs = legs,
            Version = layout.VersionNumber.ToChar(s) is char v && char.IsAsciiDigit(v) ? v - '0' : null,
            PassengerDescription = (PassengerDescription?)layout.PassengerDescription.ToChar(s),
            CheckInSource = (CheckInSource?)layout.SourceOfCheckIn.ToChar(s),
            IssuanceSource = (BoardingPassIssuanceSource?)layout.SourceOfBoardingPassIssuance.ToChar(s),
            IssueDate = issueDate,
            DocumentType = (DocumentType?)layout.DocumentType.ToChar(s),
            IssuingAirline = layout.AirlineDesignatorOfIssuer.ToTrimmedString(s),
            BaggageTagNumber = layout.BaggageTagLicensePlate.ToTrimmedString(s),
            FirstNonConsecutiveBaggageTagNumber = layout.FirstNonConsecutiveBaggageTagLicensePlate.ToTrimmedString(s),
            SecondNonConsecutiveBaggageTagNumber = layout.SecondNonConsecutiveBaggageTagLicensePlate.ToTrimmedString(s),
            Security = layout.SecurityDataIndicator.IsPresent
                ? new SecurityData(layout.SecurityDataType.ToChar(s) ?? ' ', layout.SecurityData.ToRawString(s) ?? string.Empty)
                : null,
            TrailingData = layout.TrailingData.ToTrimmedString(s),
        };

        return true;
    }

    #endregion

    #region Encoding

    /// <summary>
    /// Encodes this boarding pass as BCBP text, ready to be rendered as a PDF417, Aztec,
    /// QR or Data Matrix barcode. Conditional items without a value at the end of a section are
    /// left out, and a pass with conditional items but no <see cref="Version"/> is encoded as version 8.
    /// </summary>
    /// <exception cref="InvalidOperationException">The pass has no legs, more than 9, or a section longer than 255 characters.</exception>
    public string Encode()
    {
        var maxLength = 30 + Legs.Sum(leg => 140 + (leg.AirlineIndividualUse?.Length ?? 0)) + (Security?.Data.Length ?? 0);
        var buffer = maxLength <= 1024 ? stackalloc char[maxLength] : new char[maxLength];
        TryFormat(buffer, out var written);
        return new string(buffer[..written]);
    }

    /// <summary>Encodes this boarding pass into <paramref name="destination"/>.</summary>
    /// <returns><see langword="false"/> if <paramref name="destination"/> is too small.</returns>
    /// <exception cref="InvalidOperationException">The pass has no legs, more than 9, or a section longer than 255 characters.</exception>
    public bool TryFormat(Span<char> destination, out int charsWritten)
    {
        if (Legs.Count is < 1 or > 9)
        {
            throw new InvalidOperationException($"A boarding pass must have 1 to 9 legs but has {Legs.Count}.");
        }

        var writer = new BcbpWriter(destination);
        writer.Char(FormatCode);
        writer.Char((char)('0' + Legs.Count));
        writer.Text(PassengerName, 20);
        writer.Char(ElectronicTicketIndicator);

        for (var i = 0; i < Legs.Count; i++)
        {
            var leg = Legs[i];
            writer.Text(leg.OperatingCarrierPnrCode, 7);
            writer.Text(leg.FromAirport, 3);
            writer.Text(leg.ToAirport, 3);
            writer.Text(leg.OperatingCarrier, 3);
            writer.Text(leg.FlightNumber, 5);
            writer.Date(leg.FlightDate, withYear: false);
            writer.Char(leg.CompartmentCode);
            writer.Text(leg.SeatNumber, 4);
            writer.Text(leg.CheckInSequenceNumber, 5);
            writer.Char(leg.PassengerStatus);

            var variable = writer.BeginSection();

            if (i > 0 || Version is not null || HasStructuredConditionalItems(leg))
            {
                if (i == 0)
                {
                    WriteUniqueConditionalItems(ref writer);
                }

                WriteRepeatedConditionalItems(ref writer, leg);
            }

            writer.Text(leg.AirlineIndividualUse);
            writer.EndSection(variable);
        }

        if (Security is { } security)
        {
            writer.Char(BcbpLayout.SecurityDataIndicatorChar);
            writer.Char(security.Type);
            var section = writer.BeginSection();
            writer.Text(security.Data);
            writer.EndSection(section);
        }

        charsWritten = writer.Overflowed ? 0 : writer.Written;
        return !writer.Overflowed;
    }

    private bool HasStructuredConditionalItems(FlightLeg leg) =>
        PassengerDescription is not null || CheckInSource is not null || IssuanceSource is not null ||
        IssueDate is not null || DocumentType is not null || IssuingAirline is not null ||
        BaggageTagNumber is not null || FirstNonConsecutiveBaggageTagNumber is not null ||
        SecondNonConsecutiveBaggageTagNumber is not null ||
        leg.AirlineNumericCode is not null || leg.DocumentFormSerialNumber is not null ||
        leg.SelecteeIndicator is not null || leg.InternationalDocumentationVerification is not null ||
        leg.MarketingCarrier is not null || leg.FrequentFlyerAirline is not null ||
        leg.FrequentFlyerNumber is not null || leg.IdAdIndicator is not null ||
        leg.FreeBaggageAllowance is not null || leg.FastTrack is not null;

    private void WriteUniqueConditionalItems(ref BcbpWriter writer)
    {
        writer.Char(BcbpLayout.VersionNumberIndicatorChar);
        writer.Char((char)('0' + (Version ?? 8)));

        var unique = writer.BeginSection();
        writer.Char((char?)PassengerDescription);
        writer.Char((char?)CheckInSource);
        writer.Char((char?)IssuanceSource);
        writer.Date(IssueDate, withYear: true);
        writer.Char((char?)DocumentType);
        writer.Text(IssuingAirline, 3);
        writer.Text(BaggageTagNumber, 13);
        writer.Text(FirstNonConsecutiveBaggageTagNumber, 13);
        writer.Text(SecondNonConsecutiveBaggageTagNumber, 13);
        writer.EndSection(unique);
    }

    private static void WriteRepeatedConditionalItems(ref BcbpWriter writer, FlightLeg leg)
    {
        var repeated = writer.BeginSection();
        writer.Text(leg.AirlineNumericCode, 3);
        writer.Text(leg.DocumentFormSerialNumber, 10);
        writer.Char(leg.SelecteeIndicator);
        writer.Char((char?)leg.InternationalDocumentationVerification);
        writer.Text(leg.MarketingCarrier, 3);
        writer.Text(leg.FrequentFlyerAirline, 3);
        writer.Text(leg.FrequentFlyerNumber, 16);
        writer.Char(leg.IdAdIndicator);
        writer.Text(leg.FreeBaggageAllowance, 3);
        writer.Char(leg.FastTrack switch { true => 'Y', false => 'N', null => null });
        writer.EndSection(repeated);
    }

    #endregion
}

/// <summary>Security data that lets a third party verify the barcode has not been tampered with.</summary>
/// <param name="Type">Type of Security Data, identifying the algorithm.</param>
/// <param name="Data">The security data, typically a digital signature.</param>
public sealed record SecurityData(char Type, string Data);
