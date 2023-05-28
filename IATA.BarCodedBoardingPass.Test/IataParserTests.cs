using FluentAssertions;
using Xunit.Categories;

namespace IATA.BarCodedBoardingPass.Test
{
    public class IataParserTests
    {
        public static TheoryData<string, IataData> GetBoardingPasses()
        {
            var td = new TheoryData<string, IataData>
            {
                { 
                    "M1DOE/JOHN            EXYZ123 ZRHSFOBA 1234 147F035A0001 100", 
                    new IataData() {
                        BeginningOfMandatoryFields = "Mandatory Fields",
                        FormatCode = "M",
                        NumberOfSegments = "1",
                        PassengerName = "DOE/JOHN",
                        ElectronicTicketIndicator = "E",
                        OperatingCarrierPnrCode = "XYZ123",
                        FromCityAirportCode = "ZRH",
                        ToCityAirportCode = "SFO",
                        OperatingCarrierDesignator = "BA",
                        FlightNumber = "1234",
                        DateOfFlight = "147",
                        CompartmentCode = "F",
                        SeatNumber = "035A",
                        CheckInSequenceNumber = "0001",
                        PassengerStatus = "1",
                        ConditionalsSize = "00"
                    }
                },
                { 
                    "M1MICHEL/GEORGE       E6C2KLS ATHTORAC 1903 0114 185Y009A0013 147>218  W    B                29", 
                    new IataData() {
                        AirlineDesignatorOfIssuer = "(empty field)",
                        BaggageTagLicensePlate = "B",
                        CheckInSequenceNumber = "Y009A",
                        CompartmentCode = "4",
                        ConditionalsSize = "01",
                        DateOfFlight = "011",
                        DateOfPassIssuance = "18  ",
                        DocumentType = "W",
                        ElectronicTicketIndicator = "E",
                        FlightNumber = "1903",
                        FormatCode = "M",
                        FromCityAirportCode = "ATH",
                        NumberOfSegments = "1",
                        OperatingCarrierDesignator = "AC",
                        OperatingCarrierPnrCode = "6C2KLS",
                        PassengerDescription = "7",
                        PassengerName = "MICHEL/GEORGE",
                        PassengerStatus = "0",
                        SeatNumber = " 185",
                        SourceOfBoardingPassIssuance = "2",
                        SourceOfCheckIn = ">",
                        ToCityAirportCode = "TOR",
                        UniqueConditionalsSize = "14",
                        VersionNumber = "(empty field)"
                    }
                }
            };

            return td;
        }

        [Theory]
        [UnitTest]
        [MemberData(nameof(GetBoardingPasses))]
        public void Decode(string barcode, IataData expectedResult)
        {
            var expectedValues = expectedResult.GetProperties().Select(p => 
                (string) expectedResult.GetPropertyValue(p.Name)).Where(p => p != null).ToList();

            var result = IataParser.Decode(barcode);

            result.Should().HaveCount(60);
            result.Values.Should().AllSatisfy(x => expectedValues.Contains(x));
        }

        [Theory]
        [UnitTest]
        [MemberData(nameof(GetBoardingPasses))]
        public void DecodeObject(string barcode, IataData expectedResult)
        {
            var result = IataParser.DecodeObject(barcode);

            result.Should().BeEquivalentTo(expectedResult, options => options
                // Ignore properties containing null or whitespace
                .Excluding(p => (result.GetPropertyValue(p.Name) as string).IsNullOrWhiteSpace())
                // Override string comparison to trim extra whitespace from the result
                .Using<string>(ctx => 
                    ctx.Subject.Trim().Should().BeEquivalentTo(ctx.Expectation.Trim())).WhenTypeIs<string>());
        }
    }
}