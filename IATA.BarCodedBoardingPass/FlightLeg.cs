namespace IATA.BarCodedBoardingPass;

/// <summary>
/// One flight of a <see cref="BoardingPass"/>. Text values are as encoded, without trailing
/// padding; a value that is blank or not encoded is <see langword="null"/>.
/// </summary>
public sealed record FlightLeg
{
    /// <summary>Operating Carrier PNR Code, up to 7 characters.</summary>
    public string? OperatingCarrierPnrCode { get; init; }

    /// <summary>From City Airport Code, 3 characters.</summary>
    public string? FromAirport { get; init; }

    /// <summary>To City Airport Code, 3 characters.</summary>
    public string? ToAirport { get; init; }

    /// <summary>Operating Carrier Designator, up to 3 characters.</summary>
    public string? OperatingCarrier { get; init; }

    /// <summary>Flight Number as encoded, up to 5 characters, e.g. <c>0834</c>.</summary>
    public string? FlightNumber { get; init; }

    /// <summary>
    /// Date of Flight. The barcode only holds the day of the year; the year is inferred from
    /// the issue date when there is one, otherwise from the reference date given to
    /// <see cref="BoardingPass.Parse(string, DateOnly?)"/>.
    /// </summary>
    public DateOnly? FlightDate { get; init; }

    /// <summary>Compartment Code.</summary>
    public char? CompartmentCode { get; init; }

    /// <summary>Seat Number as encoded, up to 4 characters, e.g. <c>001A</c>.</summary>
    public string? SeatNumber { get; init; }

    /// <summary>Check-in Sequence Number as encoded, up to 5 characters.</summary>
    public string? CheckInSequenceNumber { get; init; }

    /// <summary>Passenger Status.</summary>
    public char? PassengerStatus { get; init; }

    /// <summary>Airline Numeric Code, 3 digits.</summary>
    public string? AirlineNumericCode { get; init; }

    /// <summary>Document Form/Serial Number, up to 10 characters.</summary>
    public string? DocumentFormSerialNumber { get; init; }

    /// <summary>Selectee Indicator.</summary>
    public char? SelecteeIndicator { get; init; }

    /// <summary>International Documentation Verification.</summary>
    public InternationalDocumentationVerification? InternationalDocumentationVerification { get; init; }

    /// <summary>Marketing Carrier Designator, up to 3 characters.</summary>
    public string? MarketingCarrier { get; init; }

    /// <summary>Frequent Flyer Airline Designator, up to 3 characters.</summary>
    public string? FrequentFlyerAirline { get; init; }

    /// <summary>Frequent Flyer Number, up to 16 characters.</summary>
    public string? FrequentFlyerNumber { get; init; }

    /// <summary>ID/AD Indicator.</summary>
    public char? IdAdIndicator { get; init; }

    /// <summary>Free Baggage Allowance, up to 3 characters, e.g. <c>20K</c> or <c>2PC</c>.</summary>
    public string? FreeBaggageAllowance { get; init; }

    /// <summary>Fast Track.</summary>
    public bool? FastTrack { get; init; }

    /// <summary>Data "for individual airline use" that follows the conditional items.</summary>
    public string? AirlineIndividualUse { get; init; }
}
