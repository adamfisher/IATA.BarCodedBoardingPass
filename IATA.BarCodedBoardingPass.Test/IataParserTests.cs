namespace IATA.BarCodedBoardingPass.Test;

public class IataParserTests
{
    public static TheoryData<string, Dictionary<string, string>> DecodeCases => new()
    {
        {
            TestBarcodes.DoeJohn,
            new()
            {
                ["Format Code"] = "M",
                ["Number of Legs Encoded"] = "1",
                ["Passenger Name"] = "DOE/JOHN            ",
                ["Electronic Ticket Indicator"] = "E",
                ["Operating Carrier PNR Code"] = "XYZ123 ",
                ["From City Airport Code"] = "ZRH",
                ["To City Airport Code"] = "SFO",
                ["Operating Carrier Designator"] = "BA ",
                ["Flight Number"] = "1234 ",
                ["Date of Flight (Julian Date)"] = "147",
                ["Compartment Code"] = "F",
                ["Seat Number"] = "035A",
                ["Check-in Sequence Number"] = "0001 ",
                ["Passenger Status"] = "1",
                ["Field size of variable field (Conditional + Airline item 4)"] = "00",
            }
        },
        {
            // The mandatory items are shifted, so only what they declare is decoded: a one character
            // variable size field ("3") that is not structured conditional data.
            TestBarcodes.MichelGeorgeMalformed,
            new()
            {
                ["Format Code"] = "M",
                ["Number of Legs Encoded"] = "1",
                ["Passenger Name"] = "MICHEL/GEORGE       ",
                ["Electronic Ticket Indicator"] = "E",
                ["Operating Carrier PNR Code"] = "6C2KLS ",
                ["From City Airport Code"] = "ATH",
                ["To City Airport Code"] = "TOR",
                ["Operating Carrier Designator"] = "AC ",
                ["Flight Number"] = "1903 ",
                ["Date of Flight (Julian Date)"] = "011",
                ["Compartment Code"] = "4",
                ["Seat Number"] = " 185",
                ["Check-in Sequence Number"] = "Y009A",
                ["Passenger Status"] = "0",
                ["Field size of variable field (Conditional + Airline item 4)"] = "01",
            }
        },
        {
            // Previously every field beyond position 158 was ignored and the unique conditional
            // items were assumed to be full length, which shifted the repeated items.
            TestBarcodes.BoardingPassKitSimple,
            new()
            {
                ["Format Code"] = "M",
                ["Number of Legs Encoded"] = "1",
                ["Passenger Name"] = "ACKERMANN/JUSTIN DAV",
                ["Electronic Ticket Indicator"] = "E",
                ["Operating Carrier PNR Code"] = "JKLEAJ ",
                ["From City Airport Code"] = "MSY",
                ["To City Airport Code"] = "PHX",
                ["Operating Carrier Designator"] = "AA ",
                ["Flight Number"] = "2819 ",
                ["Date of Flight (Julian Date)"] = "014",
                ["Compartment Code"] = "S",
                ["Seat Number"] = "008F",
                ["Check-in Sequence Number"] = "0059 ",
                ["Passenger Status"] = "1",
                ["Field size of variable field (Conditional + Airline item 4)"] = "4A",
                ["Version number"] = "3",
                ["Field size of following structured message - unique"] = "18",
                ["Passenger Description"] = "(empty field)",
                ["Source of check-in"] = "(empty field)",
                ["Source of Boarding Pass Issuance"] = "(empty field)",
                ["Date of Issue of Boarding Pass (Julian Date)"] = "0014",
                ["Document Type"] = "B",
                ["Airline Designator of boarding pass issuer"] = "AA ",
                ["Baggage Tag License Plate Number (s)"] = "0000000000000",
                ["Field size of following structured message - repeated"] = "29",
                ["Airline Numeric Code"] = "001",
                ["Document Form/Serial Number"] = "7484425657",
                ["Selectee Indicator"] = "3",
                ["International Document Verification"] = "(empty field)",
                ["Marketing Carrier Designator"] = "AA ",
                ["Frequent Flyer Airline Designator"] = "AA ",
                ["Frequent Flyer Number"] = "76UXK84         ",
                ["ID/AD Indicator"] = "(empty field)",
                ["Free Baggage Allowance"] = "(empty field)",
            }
        },
        {
            TestBarcodes.Version8Unspecified,
            new()
            {
                ["Format Code"] = "M",
                ["Number of Legs Encoded"] = "1",
                ["Passenger Name"] = "DOE/ALEX            ",
                ["Electronic Ticket Indicator"] = "E",
                ["Operating Carrier PNR Code"] = "ABC123 ",
                ["From City Airport Code"] = "JFK",
                ["To City Airport Code"] = "LAX",
                ["Operating Carrier Designator"] = "AA ",
                ["Flight Number"] = "0100 ",
                ["Date of Flight (Julian Date)"] = "280",
                ["Compartment Code"] = "Y",
                ["Seat Number"] = "012A",
                ["Check-in Sequence Number"] = "0001 ",
                ["Passenger Status"] = "1",
                ["Field size of variable field (Conditional + Airline item 4)"] = "1C",
                ["Version number"] = "8",
                ["Field size of following structured message - unique"] = "08",
                ["Passenger Description"] = "X",
                ["Source of check-in"] = "(empty field)",
                ["Source of Boarding Pass Issuance"] = "(empty field)",
                ["Date of Issue of Boarding Pass (Julian Date)"] = "0280",
                ["Document Type"] = "B",
                ["Field size of following structured message - repeated"] = "0E",
                ["Airline Numeric Code"] = "001",
                ["Document Form/Serial Number"] = "1234567890",
                ["Selectee Indicator"] = "0",
            }
        },
    };

    [Theory]
    [MemberData(nameof(DecodeCases))]
    public void Decode_returns_padded_values_and_leaves_absent_fields_empty(string barcode, Dictionary<string, string> expected)
    {
        var result = IataParser.Decode(barcode);

        result.Should().HaveCount(60);
        result.Should().Contain(expected);
        result.Where(entry => !expected.ContainsKey(entry.Key)).Should().AllSatisfy(entry => entry.Value.Should().BeEmpty());
    }

    [Theory]
    [MemberData(nameof(TestBarcodes.All), MemberType = typeof(TestBarcodes))]
    public void Decode_reads_every_test_barcode(string barcode)
    {
        var result = IataParser.Decode(barcode);

        result.Should().HaveCount(60);
        result["Passenger Name"].Should().NotBeEmpty();
    }

    [Fact]
    public void Decode_reads_barcodes_longer_than_158_characters()
    {
        var result = IataParser.Decode(TestBarcodes.BoardingPassKitMultiLeg);

        result["Passenger Name"].Should().Be("ACKERMANN/JUSTIN DAV");
        result["Operating Carrier PNR Code"].Should().Be("WHFPBW ");
        result["Passenger Description"].Should().Be("1");
        result["Frequent Flyer Number"].Should().Be("76UXK84         ");
    }

    [Fact]
    public void DecodeObject_trims_values()
    {
        var result = IataParser.DecodeObject(TestBarcodes.Version8Undisclosed);

        result.Should().BeEquivalentTo(new
        {
            FormatCode = "M",
            NumberOfSegments = "1",
            PassengerName = "SMITH/TAYLOR",
            ElectronicTicketIndicator = "E",
            OperatingCarrierPnrCode = "QWE123",
            FromCityAirportCode = "ATL",
            ToCityAirportCode = "CDG",
            OperatingCarrierDesignator = "DL",
            FlightNumber = "0082",
            DateOfFlight = "281",
            CompartmentCode = "J",
            SeatNumber = "002A",
            CheckInSequenceNumber = "0007",
            PassengerStatus = "1",
            ConditionalsSize = "3E",
            VersionNumber = "8",
            UniqueConditionalsSize = "0B",
            PassengerDescription = "U",
            SourceOfCheckIn = "W",
            SourceOfBoardingPassIssuance = "W",
            DateOfPassIssuance = "6280",
            DocumentType = "B",
            AirlineDesignatorOfIssuer = "DL",
            BaggageTagLicensePlate = "",
            RepeatedConditionalsSize = "2A",
            AirlineNumericCode = "006",
            SerialNumber = "0123456789",
            SelecteeIndicator = "0",
            InternationalDocumentVerification = "1",
            MarketingCarrierDesignator = "DL",
            FrequentFlyerAirlineDesignator = "DL",
            FrequentFlyerNumber = "1234567890",
            IdAdIndicator = "0",
            FreeBaggageAllowance = "1PC",
            FastTrack = "Y",
        });
    }

    [Fact]
    public void DecodeObject_keeps_the_empty_field_marker_for_blank_fields()
    {
        var result = IataParser.DecodeObject(TestBarcodes.BoardingPassKitSimple);

        result.PassengerDescription.Should().Be("(empty field)");
        result.AirlineNumericCode.Should().Be("001");
        result.FastTrack.Should().BeEmpty();
    }
}
