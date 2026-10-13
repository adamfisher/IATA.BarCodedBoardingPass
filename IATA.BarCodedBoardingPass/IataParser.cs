using System;
using System.Collections.Generic;
using System.Linq;

namespace IATA.BarCodedBoardingPass;

public static class IataParser
{
    private static readonly Dictionary<string, IataFormat> IataFormatDictionary = new()
    {
        // Mandatory
        { "BEGINNING_OF_MANDATORY_FIELDS", new IataFormat { Length = 0, Offset = 0, Content = "Mandatory Fields", Explanation = "Mandatory Fields" } },
        { "FORMAT_CODE", new IataFormat { Length = 1, Offset = 1, Content = "S|M", Explanation = "Format Code" } },
        { "NUMBER_OF_SEGMENTS", new IataFormat { Length = 1, Offset = 2, Content = "[1-4]", Explanation = "Number of Legs Encoded" } },
        { "PASSENGER_NAME", new IataFormat { Length = 20, Offset = 22, Content = "", Explanation = "Passenger Name" } },
        { "ELECTRONIC_TICKET_INDICATOR", new IataFormat { Length = 1, Offset = 23, Content = "E", Explanation = "Electronic Ticket Indicator" } },
        { "OPERATING_CARRIER_PNR_CODE", new IataFormat { Length = 7, Offset = 30, Content = "", Explanation = "Operating Carrier PNR Code" } },
        { "FROM_CITY_AIRPORT_CODE", new IataFormat { Length = 3, Offset = 33, Content = "", Explanation = "From City Airport Code" } },
        { "TO_CITY_AIRPORT_CODE", new IataFormat { Length = 3, Offset = 36, Content = "", Explanation = "To City Airport Code" } },
        { "OPERATING_CARRIER_DESIGNATOR", new IataFormat { Length = 3, Offset = 39, Content = "", Explanation = "Operating Carrier Designator" } },
        { "FLIGHT_NUMBER", new IataFormat { Length = 5, Offset = 44, Content = "", Explanation = "Flight Number" } },
        { "DATE_OF_FLIGHT", new IataFormat { Length = 3, Offset = 47, Content = "[0-9]{3}", Explanation = "Date of Flight (Julian Date)" } },
        { "COMPARTMENT_CODE", new IataFormat { Length = 1, Offset = 48, Content = "[A-Z]", Explanation = "Compartment Code" } },
        { "SEAT_NUMBER", new IataFormat { Length = 4, Offset = 52, Content = "", Explanation = "Seat Number" } },
        { "CHECK_IN_SEQUENCE_NUMBER", new IataFormat { Length = 5, Offset = 57, Content = "", Explanation = "Check-in Sequence Number" } },
        { "PASSENGER_STATUS", new IataFormat { Length = 1, Offset = 58, Content = "[0-9A-Z]", Explanation = "Passenger Status" } },
        { "CONDITIONALS_SIZE", new IataFormat { Length = 2, Offset = 60, Content = "[0-F]{2}", Explanation = "Field size of variable field (Conditional + Airline item 4)" } },
		
		// Conditional
		{ "BEGINNING_OF_VERSION_NUMBER", new IataFormat { Length = 1, Offset = 61, Content = "GREATER_THAN", Explanation = "Beginning of version number" } },
        { "VERSION_NUMBER", new IataFormat { Length = 1, Offset = 62, Content = "[1-5]", Explanation = "Version number" } },
        { "UNIQUE_CONDITIONALS_SIZE", new IataFormat { Length = 2, Offset = 64, Content = "[0-F]{2}", Explanation = "Field size of following structured message - unique" } },
        { "PASSENGER_DESCRIPTION", new IataFormat { Length = 1, Offset = 65, Content = "[0-9A-Z\\s]", Explanation = "Passenger Description" } },
        { "SOURCE_OF_CHECK_IN", new IataFormat { Length = 1, Offset = 66, Content = "[WKRMOTV\\s]", Explanation = "Source of check-in" } },
        { "SOURCE_OF_BOARDING_PASS_ISSUANCE", new IataFormat { Length = 1, Offset = 67, Content = "[WKXRMOTV\\s]", Explanation = "Source of Boarding Pass Issuance" } },
        { "DATE_OF_PASS_ISSUANCE", new IataFormat { Length = 4, Offset = 71, Content = "[0-9]{4}", Explanation = "Date of Issue of Boarding Pass (Julian Date)" } },
        { "DOCUMENT_TYPE", new IataFormat { Length = 1, Offset = 72, Content = "B|I", Explanation = "Document Type" } },
        { "AIRLINE_DESIGNATOR_OF_ISSUER", new IataFormat { Length = 3, Offset = 75, Content = "", Explanation = "Airline Designator of boarding pass issuer" } },
        { "BAGGAGE_TAG_LICENSE_PLATE", new IataFormat { Length = 13, Offset = 88, Content = "", Explanation = "Baggage Tag License Plate Number (s)" } },
        { "FIRST_BAGGAGE_TAG_LICENSE_PLATE", new IataFormat { Length = 13, Offset = 101, Content = "", Explanation = "1st Non-Consecutive Baggage Tag License Plate Number" } },
        { "SECOND_BAGGAGE_TAG_LICENSE_PLATE", new IataFormat { Length = 13, Offset = 114, Content = "", Explanation = "2nd Non-Consecutive Baggage Tag License Plate Number" } },
        { "REPEATED_CONDITIONALS_SIZE", new IataFormat { Length = 2, Offset = 116, Content = "[0-F]{2}", Explanation = "Field size of following structured message - repeated" } },
        { "AIRLINE_NUMERIC_CODE", new IataFormat { Length = 3, Offset = 119, Content = "[0-9]{3}", Explanation = "Airline Numeric Code" } },
        { "SERIAL_NUMBER", new IataFormat { Length = 10, Offset = 129, Content = "", Explanation = "Document Form/Serial Number" } },
        { "SELECTEE_INDICATOR", new IataFormat { Length = 1, Offset = 130, Content = "[\\s0-1]", Explanation = "Selectee Indicator" } },
        { "INTERNATIONAL_DOCUMENT_VERIFICATION", new IataFormat { Length = 1, Offset = 131, Content = "[\\s0-2]", Explanation = "International Document Verification" } },
        { "MARKETING_CARRIER_DESIGNATOR", new IataFormat { Length = 3, Offset = 134, Content = "", Explanation = "Marketing Carrier Designator" } },
        { "FREQUENT_FLYER_AIRLINE_DESIGNATOR", new IataFormat { Length = 3, Offset = 137, Content = "", Explanation = "Frequent Flyer Airline Designator" } },
        { "FREQUENT_FLYER_NUMBER", new IataFormat { Length = 16, Offset = 153, Content = "", Explanation = "Frequent Flyer Number" } },
        { "ID_AD_INDICATOR", new IataFormat { Length = 1, Offset = 154, Content = "[\\s0-1]", Explanation = "ID/AD Indicator" } },
        { "FREE_BAGGAGE_ALLOWANCE", new IataFormat { Length = 3, Offset = 157, Content = "", Explanation = "Free Baggage Allowance" } },
        { "FAST_TRACK", new IataFormat { Length = 1, Offset = 160, Content = "[\\s0-1]", Explanation = "Fast Track" } },
        { "AIRLINE_DESIGNATOR_OF_SECOND_CARRIER", new IataFormat { Length = 3, Offset = 161, Content = "", Explanation = "Airline Designator of Second Carrier" } },
        { "SECOND_CARRIER_DESIGNATOR_ACTION_CODE", new IataFormat { Length = 1, Offset = 164, Content = "", Explanation = "Second Carrier Designator Action Code" } },
        { "SECOND_CARRIER_NUMBER", new IataFormat { Length = 7, Offset = 165, Content = "", Explanation = "Second Carrier Number" } },
        { "BOARDING_PASS_ISSUANCE_SOURCE", new IataFormat { Length = 1, Offset = 172, Content = "[XWORM\\s]", Explanation = "Boarding Pass Issuance Source" } },
        { "ISSUING_AIRLINE_NUMERIC_CODE", new IataFormat { Length = 3, Offset = 175, Content = "[0-9]{3}", Explanation = "Issuing Airline Numeric Code" } },
        { "TO_CITY_AIRPORT_CODE_SECONDARY", new IataFormat { Length = 3, Offset = 178, Content = "", Explanation = "To City Airport Code - Secondary" } },
        { "DOCUMENT_FORM_SERIAL_NUMBER", new IataFormat { Length = 10, Offset = 191, Content = "", Explanation = "Document Form/Serial Number (MCO/MPD)" } },
        { "SELECTEE_INDICATOR_SECONDARY", new IataFormat { Length = 1, Offset = 201, Content = "[\\s0-1]", Explanation = "Selectee Indicator - Secondary" } },
        { "DATE_OF_BIRTH", new IataFormat { Length = 4, Offset = 202, Content = "[0-9]{4}", Explanation = "Date of Birth (Julian Date)" } },
        { "RESERVATION_NUMBER", new IataFormat { Length = 8, Offset = 206, Content = "", Explanation = "Reservation Number" } },
        { "OPERATIONAL_PROGRAM_VALIDATION", new IataFormat { Length = 1, Offset = 214, Content = "[\\s0-9A-Z]", Explanation = "Operational Program Validation" } },
        { "DPNA_REFERENCE_NUMBER", new IataFormat { Length = 7, Offset = 215, Content = "", Explanation = "DPNA Reference Number" } },
        { "INTERNAL_USE", new IataFormat { Length = 1, Offset = 222, Content = "[\\s0-9A-Z]", Explanation = "Internal Use" } },
        { "AIRLINE_DESIGNATOR_OF_ISSUER_SECONDARY", new IataFormat { Length = 3, Offset = 223, Content = "", Explanation = "Airline Designator of Issuer - Secondary" } },
        { "DOCUMENT_CHECKDIGIT", new IataFormat { Length = 1, Offset = 226, Content = "[0-9]", Explanation = "Document Check Digit" } },
        { "BOARDING_PASS_ISSUANCE_SOURCE_SECONDARY", new IataFormat { Length = 1, Offset = 227, Content = "[WXMORT\\s]", Explanation = "Boarding Pass Issuance Source - Secondary" } },
        { "DOCUMENT_TYPE_SECONDARY", new IataFormat { Length = 1, Offset = 228, Content = "B|I", Explanation = "Document Type - Secondary" } },
        { "SOURCE_OF_BOARDING_PASS_ISSUANCE_SECONDARY", new IataFormat { Length = 1, Offset = 229, Content = "[WXMORT\\s]", Explanation = "Source of Boarding Pass Issuance - Secondary" } },
        { "FREQUENT_FLYER_AIRLINE_DESIGNATOR_SECONDARY", new IataFormat { Length = 3, Offset = 232, Content = "", Explanation = "Frequent Flyer Airline Designator - Secondary" } },
        { "FREQUENT_FLYER_NUMBER_SECONDARY", new IataFormat { Length = 16, Offset = 248, Content = "", Explanation = "Frequent Flyer Number - Secondary" } },
        { "ID_AD_INDICATOR_SECONDARY", new IataFormat { Length = 1, Offset = 249, Content = "[\\s0-1]", Explanation = "ID/AD Indicator - Secondary" } }
    };

    /// <summary>
    /// Returns a dictionary of decoded information based on an IATA barcode.
    /// </summary>
    /// <param name="barcode">The barcode in IATA format.</param>
    /// <returns>The dictionary of decoded data.</returns>
    /// <exception cref="IataParsingException">Thrown when a parsing error occurs.</exception>
    public static IDictionary<string, string> Decode(string barcode)
    {
        var data = new Dictionary<string, string>(IataFormatDictionary.Count);

        try
        {
            foreach (var entry in IataFormatDictionary)
            {
                var key = entry.Key;
                var iataFormatObject = entry.Value;
                var value = GetEachValue(barcode, iataFormatObject);

                switch (key)
                {
                    case "BEGINNING_OF_MANDATORY_FIELDS":
                        data.Add("Mandatory Fields", string.Empty);
                        break;
                    case "BEGINNING_OF_VERSION_NUMBER":
                        data.Add("Conditional Fields", string.Empty);
                        break;
                    case "BEGINNING_OF_SECURITY_DATA":
                        data.Add("Security Fields", string.Empty);
                        break;
                    default:
                        data.Add(iataFormatObject.Explanation, value);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            throw new IataParsingException(barcode, data, ex);
        }

        return data;
    }

    /// <summary>
    /// Returns a strongly typed model of decoded information based on an IATA barcode.
    /// </summary>
    /// <param name="barcode">The barcode in IATA format.</param>
    /// <returns>The model containing the decoded data.</returns>
    /// <exception cref="IataParsingException">Thrown when a parsing error occurs.</exception>
    public static IataData DecodeObject(string barcode)
    {
        var dictionary = Decode(barcode);
        var data = new IataData();

        foreach (var entry in dictionary)
        {
            var key = IataFormatDictionary.FirstOrDefault(d => d.Value.Explanation == entry.Key).Key;

            if (key == null)
            {
                continue;
            }

            var value = entry.Value?.Trim();

            switch (key)
            {
                case "BEGINNING_OF_MANDATORY_FIELDS":
                    data.BeginningOfMandatoryFields = value;
                    break;
                case "FORMAT_CODE":
                    data.FormatCode = value;
                    break;
                case "NUMBER_OF_SEGMENTS":
                    data.NumberOfSegments = value;
                    break;
                case "PASSENGER_NAME":
                    data.PassengerName = value;
                    break;
                case "ELECTRONIC_TICKET_INDICATOR":
                    data.ElectronicTicketIndicator = value;
                    break;
                case "OPERATING_CARRIER_PNR_CODE":
                    data.OperatingCarrierPnrCode = value;
                    break;
                case "FROM_CITY_AIRPORT_CODE":
                    data.FromCityAirportCode = value;
                    break;
                case "TO_CITY_AIRPORT_CODE":
                    data.ToCityAirportCode = value;
                    break;
                case "OPERATING_CARRIER_DESIGNATOR":
                    data.OperatingCarrierDesignator = value;
                    break;
                case "FLIGHT_NUMBER":
                    data.FlightNumber = value;
                    break;
                case "DATE_OF_FLIGHT":
                    data.DateOfFlight = value;
                    break;
                case "COMPARTMENT_CODE":
                    data.CompartmentCode = value;
                    break;
                case "SEAT_NUMBER":
                    data.SeatNumber = value;
                    break;
                case "CHECK_IN_SEQUENCE_NUMBER":
                    data.CheckInSequenceNumber = value;
                    break;
                case "PASSENGER_STATUS":
                    data.PassengerStatus = value;
                    break;
                case "CONDITIONALS_SIZE":
                    data.ConditionalsSize = value;
                    break;
                case "BEGINNING_OF_VERSION_NUMBER":
                    data.BeginningOfVersionNumber = value;
                    break;
                case "VERSION_NUMBER":
                    data.VersionNumber = value;
                    break;
                case "UNIQUE_CONDITIONALS_SIZE":
                    data.UniqueConditionalsSize = value;
                    break;
                case "PASSENGER_DESCRIPTION":
                    data.PassengerDescription = value;
                    break;
                case "SOURCE_OF_CHECK_IN":
                    data.SourceOfCheckIn = value;
                    break;
                case "SOURCE_OF_BOARDING_PASS_ISSUANCE":
                    data.SourceOfBoardingPassIssuance = value;
                    break;
                case "DATE_OF_PASS_ISSUANCE":
                    data.DateOfPassIssuance = value;
                    break;
                case "DOCUMENT_TYPE":
                    data.DocumentType = value;
                    break;
                case "AIRLINE_DESIGNATOR_OF_ISSUER":
                    data.AirlineDesignatorOfIssuer = value;
                    break;
                case "BAGGAGE_TAG_LICENSE_PLATE":
                    data.BaggageTagLicensePlate = value;
                    break;
                case "FIRST_BAGGAGE_TAG_LICENSE_PLATE":
                    data.FirstBaggageTagLicensePlate = value;
                    break;
                case "SECOND_BAGGAGE_TAG_LICENSE_PLATE":
                    data.SecondBaggageTagLicensePlate = value;
                    break;
                case "REPEATED_CONDITIONALS_SIZE":
                    data.RepeatedConditionalsSize = value;
                    break;
                case "AIRLINE_NUMERIC_CODE":
                    data.AirlineNumericCode = value;
                    break;
                case "SERIAL_NUMBER":
                    data.SerialNumber = value;
                    break;
                case "SELECTEE_INDICATOR":
                    data.SelecteeIndicator = value;
                    break;
                case "INTERNATIONAL_DOCUMENT_VERIFICATION":
                    data.InternationalDocumentVerification = value;
                    break;
                case "MARKETING_CARRIER_DESIGNATOR":
                    data.MarketingCarrierDesignator = value;
                    break;
                case "FREQUENT_FLYER_AIRLINE_DESIGNATOR":
                    data.FrequentFlyerAirlineDesignator = value;
                    break;
                case "FREQUENT_FLYER_NUMBER":
                    data.FrequentFlyerNumber = value;
                    break;
                case "ID_AD_INDICATOR":
                    data.IdAdIndicator = value;
                    break;
                case "FREE_BAGGAGE_ALLOWANCE":
                    data.FreeBaggageAllowance = value;
                    break;
                case "FAST_TRACK":
                    data.FastTrack = value;
                    break;
                case "AIRLINE_DESIGNATOR_OF_SECOND_CARRIER":
                    data.AirlineDesignatorOfSecondCarrier = value;
                    break;
                case "SECOND_CARRIER_DESIGNATOR_ACTION_CODE":
                    data.SecondCarrierDesignatorActionCode = value;
                    break;
                case "SECOND_CARRIER_NUMBER":
                    data.SecondCarrierNumber = value;
                    break;
                case "BOARDING_PASS_ISSUANCE_SOURCE":
                    data.BoardingPassIssuanceSource = value;
                    break;
                case "ISSUING_AIRLINE_NUMERIC_CODE":
                    data.IssuingAirlineNumericCode = value;
                    break;
                case "TO_CITY_AIRPORT_CODE_SECONDARY":
                    data.ToCityAirportCodeSecondary = value;
                    break;
                case "DOCUMENT_FORM_SERIAL_NUMBER":
                    data.DocumentFormSerialNumber = value;
                    break;
                case "SELECTEE_INDICATOR_SECONDARY":
                    data.SelecteeIndicatorSecondary = value;
                    break;
                case "DATE_OF_BIRTH":
                    data.DateOfBirth = value;
                    break;
                case "RESERVATION_NUMBER":
                    data.ReservationNumber = value;
                    break;
                case "OPERATIONAL_PROGRAM_VALIDATION":
                    data.OperationalProgramValidation = value;
                    break;
                case "DPNA_REFERENCE_NUMBER":
                    data.DpnaReferenceNumber = value;
                    break;
                case "INTERNAL_USE":
                    data.InternalUse = value;
                    break;
                case "AIRLINE_DESIGNATOR_OF_ISSUER_SECONDARY":
                    data.AirlineDesignatorOfIssuerSecondary = value;
                    break;
                case "DOCUMENT_CHECK_DIGIT":
                    data.DocumentCheckDigit = value;
                    break;
                case "BOARDING_PASS_ISSUANCE_SOURCE_SECONDARY":
                    data.BoardingPassIssuanceSourceSecondary = value;
                    break;
                case "DOCUMENT_TYPE_SECONDARY":
                    data.DocumentTypeSecondary = value;
                    break;
                case "FREQUENT_FLYER_COMPARTMENT_CODE":
                    data.FrequentFlyerCompartmentCode = value;
                    break;
                case "SOURCE_OF_BOARDING_PASS_ISSUANCE_SECONDARY":
                    data.SourceOfBoardingPassIssuanceSecondary = value;
                    break;
                case "FREQUENT_FLYER_AIRLINE_DESIGNATOR_SECONDARY":
                    data.FrequentFlyerAirlineDesignatorSecondary = value;
                    break;
                case "FREQUENT_FLYER_NUMBER_SECONDARY":
                    data.FrequentFlyerNumberSecondary = value;
                    break;
                case "ID_AD_INDICATOR_SECONDARY":
                    data.IdAdIndicatorSecondary = value;
                    break;
            }
        }

        return data;
    }

    private static string GetEachValue(string input, IataFormat iataFormatObject)
    {
        if (iataFormatObject.Offset <= input.Length && input.Length <= 158) // because of airline's var field, see below
        {
            var tmp = input.Substring(iataFormatObject.Offset - iataFormatObject.Length, iataFormatObject.Length);
            return tmp.Trim().Length == 0 ? "(empty field)" : tmp;
        }
        return string.Empty;
    }
}