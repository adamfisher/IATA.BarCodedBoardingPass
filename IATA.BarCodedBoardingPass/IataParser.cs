using IATA.BarCodedBoardingPass.Internal;

namespace IATA.BarCodedBoardingPass;

/// <summary>
/// Decodes IATA Bar Coded Boarding Pass (BCBP) strings into a dictionary or an <see cref="IataData"/>.
/// </summary>
/// <remarks>
/// Only the first leg of a multi-leg boarding pass is returned.
/// </remarks>
public static class IataParser
{
    private const string EmptyField = "(empty field)";

    private sealed record Field(
        string Explanation,
        Func<BcbpLayout, LegLayout?, FieldSpan> Select,
        Action<IataData, string>? Assign);

    private static FieldSpan None(BcbpLayout pass, LegLayout? leg) => FieldSpan.Missing;

    private static readonly Field[] Fields =
    [
        // Mandatory
        new("Mandatory Fields", None, (d, v) => d.BeginningOfMandatoryFields = v),
        new("Format Code", (p, _) => p.FormatCode, (d, v) => d.FormatCode = v),
        new("Number of Legs Encoded", (p, _) => p.NumberOfLegs, (d, v) => d.NumberOfSegments = v),
        new("Passenger Name", (p, _) => p.PassengerName, (d, v) => d.PassengerName = v),
        new("Electronic Ticket Indicator", (p, _) => p.ElectronicTicketIndicator, (d, v) => d.ElectronicTicketIndicator = v),
        new("Operating Carrier PNR Code", (_, l) => l?.OperatingCarrierPnrCode ?? FieldSpan.Missing, (d, v) => d.OperatingCarrierPnrCode = v),
        new("From City Airport Code", (_, l) => l?.FromCityAirportCode ?? FieldSpan.Missing, (d, v) => d.FromCityAirportCode = v),
        new("To City Airport Code", (_, l) => l?.ToCityAirportCode ?? FieldSpan.Missing, (d, v) => d.ToCityAirportCode = v),
        new("Operating Carrier Designator", (_, l) => l?.OperatingCarrierDesignator ?? FieldSpan.Missing, (d, v) => d.OperatingCarrierDesignator = v),
        new("Flight Number", (_, l) => l?.FlightNumber ?? FieldSpan.Missing, (d, v) => d.FlightNumber = v),
        new("Date of Flight (Julian Date)", (_, l) => l?.DateOfFlight ?? FieldSpan.Missing, (d, v) => d.DateOfFlight = v),
        new("Compartment Code", (_, l) => l?.CompartmentCode ?? FieldSpan.Missing, (d, v) => d.CompartmentCode = v),
        new("Seat Number", (_, l) => l?.SeatNumber ?? FieldSpan.Missing, (d, v) => d.SeatNumber = v),
        new("Check-in Sequence Number", (_, l) => l?.CheckInSequenceNumber ?? FieldSpan.Missing, (d, v) => d.CheckInSequenceNumber = v),
        new("Passenger Status", (_, l) => l?.PassengerStatus ?? FieldSpan.Missing, (d, v) => d.PassengerStatus = v),
        new("Field size of variable field (Conditional + Airline item 4)", (_, l) => l?.ConditionalsSize ?? FieldSpan.Missing, (d, v) => d.ConditionalsSize = v),

        // Conditional
        new("Conditional Fields", None, null),
        new("Version number", (p, _) => p.VersionNumber, (d, v) => d.VersionNumber = v),
        new("Field size of following structured message - unique", (p, _) => p.UniqueConditionalsSize, (d, v) => d.UniqueConditionalsSize = v),
        new("Passenger Description", (p, _) => p.PassengerDescription, (d, v) => d.PassengerDescription = v),
        new("Source of check-in", (p, _) => p.SourceOfCheckIn, (d, v) => d.SourceOfCheckIn = v),
        new("Source of Boarding Pass Issuance", (p, _) => p.SourceOfBoardingPassIssuance, (d, v) => d.SourceOfBoardingPassIssuance = v),
        new("Date of Issue of Boarding Pass (Julian Date)", (p, _) => p.DateOfIssue, (d, v) => d.DateOfPassIssuance = v),
        new("Document Type", (p, _) => p.DocumentType, (d, v) => d.DocumentType = v),
        new("Airline Designator of boarding pass issuer", (p, _) => p.AirlineDesignatorOfIssuer, (d, v) => d.AirlineDesignatorOfIssuer = v),
        new("Baggage Tag License Plate Number (s)", (p, _) => p.BaggageTagLicensePlate, (d, v) => d.BaggageTagLicensePlate = v),
        new("1st Non-Consecutive Baggage Tag License Plate Number", (p, _) => p.FirstNonConsecutiveBaggageTagLicensePlate, (d, v) => d.FirstBaggageTagLicensePlate = v),
        new("2nd Non-Consecutive Baggage Tag License Plate Number", (p, _) => p.SecondNonConsecutiveBaggageTagLicensePlate, (d, v) => d.SecondBaggageTagLicensePlate = v),
        new("Field size of following structured message - repeated", (_, l) => l?.RepeatedConditionalsSize ?? FieldSpan.Missing, (d, v) => d.RepeatedConditionalsSize = v),
        new("Airline Numeric Code", (_, l) => l?.AirlineNumericCode ?? FieldSpan.Missing, (d, v) => d.AirlineNumericCode = v),
        new("Document Form/Serial Number", (_, l) => l?.DocumentFormSerialNumber ?? FieldSpan.Missing, (d, v) => d.SerialNumber = v),
        new("Selectee Indicator", (_, l) => l?.SelecteeIndicator ?? FieldSpan.Missing, (d, v) => d.SelecteeIndicator = v),
        new("International Document Verification", (_, l) => l?.InternationalDocumentVerification ?? FieldSpan.Missing, (d, v) => d.InternationalDocumentVerification = v),
        new("Marketing Carrier Designator", (_, l) => l?.MarketingCarrierDesignator ?? FieldSpan.Missing, (d, v) => d.MarketingCarrierDesignator = v),
        new("Frequent Flyer Airline Designator", (_, l) => l?.FrequentFlyerAirlineDesignator ?? FieldSpan.Missing, (d, v) => d.FrequentFlyerAirlineDesignator = v),
        new("Frequent Flyer Number", (_, l) => l?.FrequentFlyerNumber ?? FieldSpan.Missing, (d, v) => d.FrequentFlyerNumber = v),
        new("ID/AD Indicator", (_, l) => l?.IdAdIndicator ?? FieldSpan.Missing, (d, v) => d.IdAdIndicator = v),
        new("Free Baggage Allowance", (_, l) => l?.FreeBaggageAllowance ?? FieldSpan.Missing, (d, v) => d.FreeBaggageAllowance = v),
        new("Fast Track", (_, l) => l?.FastTrack ?? FieldSpan.Missing, (d, v) => d.FastTrack = v),

        // The remaining keys are not IATA BCBP items. They are kept so existing dictionary
        // consumers still find them, but they are always empty.
        new("Airline Designator of Second Carrier", None, null),
        new("Second Carrier Designator Action Code", None, null),
        new("Second Carrier Number", None, null),
        new("Boarding Pass Issuance Source", None, null),
        new("Issuing Airline Numeric Code", None, null),
        new("To City Airport Code - Secondary", None, null),
        new("Document Form/Serial Number (MCO/MPD)", None, null),
        new("Selectee Indicator - Secondary", None, null),
        new("Date of Birth (Julian Date)", None, null),
        new("Reservation Number", None, null),
        new("Operational Program Validation", None, null),
        new("DPNA Reference Number", None, null),
        new("Internal Use", None, null),
        new("Airline Designator of Issuer - Secondary", None, null),
        new("Document Check Digit", None, null),
        new("Boarding Pass Issuance Source - Secondary", None, null),
        new("Document Type - Secondary", None, null),
        new("Source of Boarding Pass Issuance - Secondary", None, null),
        new("Frequent Flyer Airline Designator - Secondary", None, null),
        new("Frequent Flyer Number - Secondary", None, null),
        new("ID/AD Indicator - Secondary", None, null),
    ];

    /// <summary>
    /// Returns a dictionary of decoded information based on an IATA barcode.
    /// </summary>
    /// <param name="barcode">The barcode in IATA format.</param>
    /// <returns>
    /// The decoded data keyed by field description. Values keep their space padding; a field
    /// that is present but blank is <c>"(empty field)"</c> and a field that is not present is empty.
    /// </returns>
    /// <exception cref="IataParsingException">Thrown when a parsing error occurs.</exception>
    public static IDictionary<string, string> Decode(string barcode)
    {
        var data = new Dictionary<string, string>(Fields.Length);

        if (!BcbpLayout.TryRead(barcode, out var layout, out var error))
        {
            throw new IataParsingException(barcode, data, error.ToException());
        }

        var leg = layout.Legs.Count > 0 ? layout.Legs[0] : null;

        foreach (var field in Fields)
        {
            data.Add(field.Explanation, Format(field.Select(layout, leg), barcode));
        }

        return data;
    }

    /// <summary>
    /// Returns a strongly typed model of decoded information based on an IATA barcode.
    /// </summary>
    /// <param name="barcode">The barcode in IATA format.</param>
    /// <returns>The model containing the decoded data, with values trimmed.</returns>
    /// <exception cref="IataParsingException">Thrown when a parsing error occurs.</exception>
    public static IataData DecodeObject(string barcode)
    {
        var dictionary = Decode(barcode);
        var data = new IataData();

        foreach (var field in Fields)
        {
            field.Assign?.Invoke(data, dictionary[field.Explanation].Trim());
        }

        return data;
    }

    private static string Format(FieldSpan field, string barcode) => field switch
    {
        { IsPresent: false } => string.Empty,
        _ when field.Slice(barcode).IsWhiteSpace() => EmptyField,
        _ => field.ToRawString(barcode)!,
    };
}
