namespace IATA.BarCodedBoardingPass;

/// <summary>
/// Decoded boarding pass fields returned by <see cref="IataParser.DecodeObject"/>. Values are
/// trimmed strings exactly as encoded; only the first leg of a multi-leg pass is included.
/// </summary>
public class IataData
{
    private const string NotInStandard = "Not an IATA BCBP item; always null.";

    /// <summary>Always empty; marks the start of the mandatory items.</summary>
    public string? BeginningOfMandatoryFields { get; set; }

    /// <summary>Format Code, normally <c>M</c>.</summary>
    public string? FormatCode { get; set; }

    /// <summary>Number of Legs Encoded (1-4).</summary>
    public string? NumberOfSegments { get; set; }

    /// <summary>Passenger Name as <c>LAST/FIRST</c>.</summary>
    public string? PassengerName { get; set; }

    /// <summary>Electronic Ticket Indicator, <c>E</c> for an e-ticket.</summary>
    public string? ElectronicTicketIndicator { get; set; }

    /// <summary>Operating Carrier PNR Code.</summary>
    public string? OperatingCarrierPnrCode { get; set; }

    /// <summary>From City Airport Code.</summary>
    public string? FromCityAirportCode { get; set; }

    /// <summary>To City Airport Code.</summary>
    public string? ToCityAirportCode { get; set; }

    /// <summary>Operating Carrier Designator.</summary>
    public string? OperatingCarrierDesignator { get; set; }

    /// <summary>Flight Number.</summary>
    public string? FlightNumber { get; set; }

    /// <summary>Date of Flight as a three digit day of the year.</summary>
    public string? DateOfFlight { get; set; }

    /// <summary>Compartment Code.</summary>
    public string? CompartmentCode { get; set; }

    /// <summary>Seat Number.</summary>
    public string? SeatNumber { get; set; }

    /// <summary>Check-in Sequence Number.</summary>
    public string? CheckInSequenceNumber { get; set; }

    /// <summary>Passenger Status.</summary>
    public string? PassengerStatus { get; set; }

    /// <summary>Field size of the variable size field, in hexadecimal.</summary>
    public string? ConditionalsSize { get; set; }

    /// <summary>Not populated.</summary>
    public string? BeginningOfVersionNumber { get; set; }

    /// <summary>Version Number of the BCBP standard the pass was encoded with.</summary>
    public string? VersionNumber { get; set; }

    /// <summary>Field size of the unique conditional items, in hexadecimal.</summary>
    public string? UniqueConditionalsSize { get; set; }

    /// <summary>
    /// Passenger Description: <c>0</c> adult, <c>1</c> male, <c>2</c> female, <c>3</c> child,
    /// <c>4</c> infant, <c>5</c> no passenger (cabin baggage), <c>6</c> adult travelling with infant,
    /// <c>7</c> unaccompanied minor, <c>X</c> unspecified, <c>U</c> undisclosed.
    /// </summary>
    public string? PassengerDescription { get; set; }

    /// <summary>Source of check-in.</summary>
    public string? SourceOfCheckIn { get; set; }

    /// <summary>Source of Boarding Pass Issuance.</summary>
    public string? SourceOfBoardingPassIssuance { get; set; }

    /// <summary>Date of Issue of Boarding Pass: last digit of the year followed by the day of the year.</summary>
    public string? DateOfPassIssuance { get; set; }

    /// <summary>Document Type: <c>B</c> boarding pass, <c>I</c> itinerary receipt.</summary>
    public string? DocumentType { get; set; }

    /// <summary>Airline Designator of boarding pass issuer.</summary>
    public string? AirlineDesignatorOfIssuer { get; set; }

    /// <summary>Baggage Tag License Plate Number(s).</summary>
    public string? BaggageTagLicensePlate { get; set; }

    /// <summary>1st Non-Consecutive Baggage Tag License Plate Number.</summary>
    public string? FirstBaggageTagLicensePlate { get; set; }

    /// <summary>2nd Non-Consecutive Baggage Tag License Plate Number.</summary>
    public string? SecondBaggageTagLicensePlate { get; set; }

    /// <summary>Field size of the repeated conditional items, in hexadecimal.</summary>
    public string? RepeatedConditionalsSize { get; set; }

    /// <summary>Airline Numeric Code.</summary>
    public string? AirlineNumericCode { get; set; }

    /// <summary>Document Form/Serial Number.</summary>
    public string? SerialNumber { get; set; }

    /// <summary>Selectee Indicator.</summary>
    public string? SelecteeIndicator { get; set; }

    /// <summary>International Documentation Verification.</summary>
    public string? InternationalDocumentVerification { get; set; }

    /// <summary>Marketing Carrier Designator.</summary>
    public string? MarketingCarrierDesignator { get; set; }

    /// <summary>Frequent Flyer Airline Designator.</summary>
    public string? FrequentFlyerAirlineDesignator { get; set; }

    /// <summary>Frequent Flyer Number.</summary>
    public string? FrequentFlyerNumber { get; set; }

    /// <summary>ID/AD Indicator.</summary>
    public string? IdAdIndicator { get; set; }

    /// <summary>Free Baggage Allowance.</summary>
    public string? FreeBaggageAllowance { get; set; }

    /// <summary>Fast Track, <c>Y</c> or <c>N</c>.</summary>
    public string? FastTrack { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? AirlineDesignatorOfSecondCarrier { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? SecondCarrierDesignatorActionCode { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? SecondCarrierNumber { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? BoardingPassIssuanceSource { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? IssuingAirlineNumericCode { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? ToCityAirportCodeSecondary { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? DocumentFormSerialNumber { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? SelecteeIndicatorSecondary { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? DateOfBirth { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? ReservationNumber { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? OperationalProgramValidation { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? DpnaReferenceNumber { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? InternalUse { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? AirlineDesignatorOfIssuerSecondary { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? DocumentCheckDigit { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? BoardingPassIssuanceSourceSecondary { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? DocumentTypeSecondary { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? FrequentFlyerCompartmentCode { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? SourceOfBoardingPassIssuanceSecondary { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? FrequentFlyerAirlineDesignatorSecondary { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? FrequentFlyerNumberSecondary { get; set; }

    /// <summary>Not an IATA BCBP item; always null.</summary>
    [Obsolete(NotInStandard)] public string? IdAdIndicatorSecondary { get; set; }
}
