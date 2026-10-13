using System.Diagnostics.CodeAnalysis;

namespace IATA.BarCodedBoardingPass.Internal;

/// <summary>
/// The position of every field in a BCBP string, found by following the field size items
/// that IATA Resolution 792 uses to describe each variable length section.
/// </summary>
/// <remarks>
/// <code>
/// Format Code | Number of Legs | Passenger Name | Electronic Ticket Indicator
/// for each leg:
///     repeated mandatory items | Field Size of variable size field (hex)
///     variable size field:
///         first leg only: '>' | Version Number | Field Size (hex) | unique conditional items
///         Field Size (hex) | repeated conditional items
///         airline individual use
/// '^' | Type of Security Data | Length of Security Data (hex) | Security Data
/// </code>
/// </remarks>
internal sealed class BcbpLayout
{
    public const char VersionNumberIndicatorChar = '>';
    public const char SecurityDataIndicatorChar = '^';

    public FieldSpan FormatCode { get; private set; }
    public FieldSpan NumberOfLegs { get; private set; }
    public FieldSpan PassengerName { get; private set; }
    public FieldSpan ElectronicTicketIndicator { get; private set; }
    public List<LegLayout> Legs { get; } = [];

    // Conditional items that appear once, inside the first leg's variable size field.
    public FieldSpan VersionNumberIndicator { get; private set; } = FieldSpan.Missing;
    public FieldSpan VersionNumber { get; private set; } = FieldSpan.Missing;
    public FieldSpan UniqueConditionalsSize { get; private set; } = FieldSpan.Missing;
    public FieldSpan PassengerDescription { get; private set; } = FieldSpan.Missing;
    public FieldSpan SourceOfCheckIn { get; private set; } = FieldSpan.Missing;
    public FieldSpan SourceOfBoardingPassIssuance { get; private set; } = FieldSpan.Missing;
    public FieldSpan DateOfIssue { get; private set; } = FieldSpan.Missing;
    public FieldSpan DocumentType { get; private set; } = FieldSpan.Missing;
    public FieldSpan AirlineDesignatorOfIssuer { get; private set; } = FieldSpan.Missing;
    public FieldSpan BaggageTagLicensePlate { get; private set; } = FieldSpan.Missing;
    public FieldSpan FirstNonConsecutiveBaggageTagLicensePlate { get; private set; } = FieldSpan.Missing;
    public FieldSpan SecondNonConsecutiveBaggageTagLicensePlate { get; private set; } = FieldSpan.Missing;

    public FieldSpan SecurityDataIndicator { get; private set; } = FieldSpan.Missing;
    public FieldSpan SecurityDataType { get; private set; } = FieldSpan.Missing;
    public FieldSpan SecurityDataLength { get; private set; } = FieldSpan.Missing;
    public FieldSpan SecurityData { get; private set; } = FieldSpan.Missing;

    /// <summary>Anything after the last leg and the security data.</summary>
    public FieldSpan TrailingData { get; private set; } = FieldSpan.Missing;

    public static bool TryRead(
        ReadOnlySpan<char> barcode,
        [NotNullWhen(true)] out BcbpLayout? layout,
        out ParseError error)
    {
        layout = null;

        if (barcode.Length < 2)
        {
            error = new ParseError("The barcode is too short to be a boarding pass.", 0);
            return false;
        }

        if (barcode[1] is < '1' or > '9')
        {
            error = new ParseError($"Number of Legs Encoded must be a digit from 1 to 9 but was '{barcode[1]}'.", 1);
            return false;
        }

        var main = new SectionCursor(barcode, 0, barcode.Length);
        var result = new BcbpLayout
        {
            FormatCode = main.Read(1),
            NumberOfLegs = main.Read(1),
            PassengerName = main.Read(20),
            ElectronicTicketIndicator = main.Read(1),
        };

        for (var legIndex = 0; legIndex < barcode[1] - '0'; legIndex++)
        {
            var leg = new LegLayout
            {
                OperatingCarrierPnrCode = main.Read(7),
                FromCityAirportCode = main.Read(3),
                ToCityAirportCode = main.Read(3),
                OperatingCarrierDesignator = main.Read(3),
                FlightNumber = main.Read(5),
                DateOfFlight = main.Read(3),
                CompartmentCode = main.Read(1),
                SeatNumber = main.Read(4),
                CheckInSequenceNumber = main.Read(5),
                PassengerStatus = main.Read(1),
            };

            if (!main.TryReadSection("Field size of variable size field", out var variable, out var variableSize, out error) ||
                !result.TryReadVariableSizeField(ref variable, leg, isFirstLeg: legIndex == 0, out error))
            {
                return false;
            }

            leg.ConditionalsSize = variableSize;
            result.Legs.Add(leg);
        }

        if (main.Peek() == SecurityDataIndicatorChar)
        {
            result.SecurityDataIndicator = main.Read(1);
            result.SecurityDataType = main.Read(1);

            if (!main.TryReadSection("Length of Security Data", out var security, out var securityLength, out error))
            {
                return false;
            }

            result.SecurityDataLength = securityLength;
            result.SecurityData = security.ReadToEnd();
        }

        result.TrailingData = main.ReadToEnd();
        layout = result;
        error = default;
        return true;
    }

    private bool TryReadVariableSizeField(ref SectionCursor variable, LegLayout leg, bool isFirstLeg, out ParseError error)
    {
        error = default;

        if (variable.IsAtEnd)
        {
            return true;
        }

        if (isFirstLeg)
        {
            if (variable.Peek() != VersionNumberIndicatorChar)
            {
                // Not structured conditional data; keep it rather than guess at its layout.
                leg.AirlineIndividualUse = variable.ReadToEnd();
                return true;
            }

            VersionNumberIndicator = variable.Read(1);
            VersionNumber = variable.Read(1);

            if (!variable.TryReadSection("Field size of following structured message - unique", out var unique, out var uniqueSize, out error))
            {
                return false;
            }

            UniqueConditionalsSize = uniqueSize;
            PassengerDescription = unique.Read(1);
            SourceOfCheckIn = unique.Read(1);
            SourceOfBoardingPassIssuance = unique.Read(1);
            DateOfIssue = unique.Read(4);
            DocumentType = unique.Read(1);
            AirlineDesignatorOfIssuer = unique.Read(3);
            BaggageTagLicensePlate = unique.Read(13);
            FirstNonConsecutiveBaggageTagLicensePlate = unique.Read(13);
            SecondNonConsecutiveBaggageTagLicensePlate = unique.Read(13);
        }

        if (!variable.TryReadSection("Field size of following structured message - repeated", out var repeated, out var repeatedSize, out error))
        {
            return false;
        }

        leg.RepeatedConditionalsSize = repeatedSize;
        leg.AirlineNumericCode = repeated.Read(3);
        leg.DocumentFormSerialNumber = repeated.Read(10);
        leg.SelecteeIndicator = repeated.Read(1);
        leg.InternationalDocumentVerification = repeated.Read(1);
        leg.MarketingCarrierDesignator = repeated.Read(3);
        leg.FrequentFlyerAirlineDesignator = repeated.Read(3);
        leg.FrequentFlyerNumber = repeated.Read(16);
        leg.IdAdIndicator = repeated.Read(1);
        leg.FreeBaggageAllowance = repeated.Read(3);
        leg.FastTrack = repeated.Read(1);

        leg.AirlineIndividualUse = variable.ReadToEnd();
        return true;
    }
}

/// <summary>The position of every field of one flight leg.</summary>
internal sealed class LegLayout
{
    public FieldSpan OperatingCarrierPnrCode { get; init; }
    public FieldSpan FromCityAirportCode { get; init; }
    public FieldSpan ToCityAirportCode { get; init; }
    public FieldSpan OperatingCarrierDesignator { get; init; }
    public FieldSpan FlightNumber { get; init; }
    public FieldSpan DateOfFlight { get; init; }
    public FieldSpan CompartmentCode { get; init; }
    public FieldSpan SeatNumber { get; init; }
    public FieldSpan CheckInSequenceNumber { get; init; }
    public FieldSpan PassengerStatus { get; init; }
    public FieldSpan ConditionalsSize { get; set; } = FieldSpan.Missing;

    public FieldSpan RepeatedConditionalsSize { get; set; } = FieldSpan.Missing;
    public FieldSpan AirlineNumericCode { get; set; } = FieldSpan.Missing;
    public FieldSpan DocumentFormSerialNumber { get; set; } = FieldSpan.Missing;
    public FieldSpan SelecteeIndicator { get; set; } = FieldSpan.Missing;
    public FieldSpan InternationalDocumentVerification { get; set; } = FieldSpan.Missing;
    public FieldSpan MarketingCarrierDesignator { get; set; } = FieldSpan.Missing;
    public FieldSpan FrequentFlyerAirlineDesignator { get; set; } = FieldSpan.Missing;
    public FieldSpan FrequentFlyerNumber { get; set; } = FieldSpan.Missing;
    public FieldSpan IdAdIndicator { get; set; } = FieldSpan.Missing;
    public FieldSpan FreeBaggageAllowance { get; set; } = FieldSpan.Missing;
    public FieldSpan FastTrack { get; set; } = FieldSpan.Missing;

    /// <summary>"For individual airline use" data after the repeated conditional items.</summary>
    public FieldSpan AirlineIndividualUse { get; set; } = FieldSpan.Missing;
}
