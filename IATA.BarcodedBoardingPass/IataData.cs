﻿namespace IATA.BarCodedBoardingPass;

public class IataData
{
    public string BeginningOfMandatoryFields { get; set; }
    public string FormatCode { get; set; }
    public string NumberOfSegments { get; set; }
    public string PassengerName { get; set; }
    public string ElectronicTicketIndicator { get; set; }
    public string OperatingCarrierPnrCode { get; set; }
    public string FromCityAirportCode { get; set; }
    public string ToCityAirportCode { get; set; }
    public string OperatingCarrierDesignator { get; set; }
    public string FlightNumber { get; set; }
    public string DateOfFlight { get; set; }
    public string CompartmentCode { get; set; }
    public string SeatNumber { get; set; }
    public string CheckInSequenceNumber { get; set; }
    public string PassengerStatus { get; set; }
    public string ConditionalsSize { get; set; }
    public string BeginningOfVersionNumber { get; set; }
    public string VersionNumber { get; set; }
    public string UniqueConditionalsSize { get; set; }
    public string PassengerDescription { get; set; }
    public string SourceOfCheckIn { get; set; }
    public string SourceOfBoardingPassIssuance { get; set; }
    public string DateOfPassIssuance { get; set; }
    public string DocumentType { get; set; }
    public string AirlineDesignatorOfIssuer { get; set; }
    public string BaggageTagLicensePlate { get; set; }
    public string FirstBaggageTagLicensePlate { get; set; }
    public string SecondBaggageTagLicensePlate { get; set; }
    public string RepeatedConditionalsSize { get; set; }
    public string AirlineNumericCode { get; set; }
    public string SerialNumber { get; set; }
    public string SelecteeIndicator { get; set; }
    public string InternationalDocumentVerification { get; set; }
    public string MarketingCarrierDesignator { get; set; }
    public string FrequentFlyerAirlineDesignator { get; set; }
    public string FrequentFlyerNumber { get; set; }
    public string IdAdIndicator { get; set; }
    public string FreeBaggageAllowance { get; set; }
    public string FastTrack { get; set; }
    public string AirlineDesignatorOfSecondCarrier { get; set; }
    public string SecondCarrierDesignatorActionCode { get; set; }
    public string SecondCarrierNumber { get; set; }
    public string BoardingPassIssuanceSource { get; set; }
    public string IssuingAirlineNumericCode { get; set; }
    public string ToCityAirportCodeSecondary { get; set; }
    public string DocumentFormSerialNumber { get; set; }
    public string SelecteeIndicatorSecondary { get; set; }
    public string DateOfBirth { get; set; }
    public string ReservationNumber { get; set; }
    public string OperationalProgramValidation { get; set; }
    public string DpnaReferenceNumber { get; set; }
    public string InternalUse { get; set; }
    public string AirlineDesignatorOfIssuerSecondary { get; set; }
    public string DocumentCheckDigit { get; set; }
    public string BoardingPassIssuanceSourceSecondary { get; set; }
    public string DocumentTypeSecondary { get; set; }
    public string FrequentFlyerCompartmentCode { get; set; }
    public string SourceOfBoardingPassIssuanceSecondary { get; set; }
    public string FrequentFlyerAirlineDesignatorSecondary { get; set; }
    public string FrequentFlyerNumberSecondary { get; set; }
    public string IdAdIndicatorSecondary { get; set; }
}