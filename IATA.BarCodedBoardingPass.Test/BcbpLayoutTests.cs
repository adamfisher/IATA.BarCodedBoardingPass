using IATA.BarCodedBoardingPass.Internal;

namespace IATA.BarCodedBoardingPass.Test;

public class BcbpLayoutTests
{
    private static BcbpLayout Read(string barcode)
    {
        BcbpLayout.TryRead(barcode, out var layout, out var error).Should().BeTrue(error.Message);
        return layout!;
    }

    [Theory]
    [MemberData(nameof(TestBarcodes.All), MemberType = typeof(TestBarcodes))]
    public void Reads_every_test_barcode(string barcode)
    {
        var layout = Read(barcode);

        layout.Legs.Should().HaveCount(barcode[1] - '0');
    }

    [Fact]
    public void Reads_each_leg_of_a_multi_leg_pass()
    {
        const string barcode = TestBarcodes.BcbpFull;
        var layout = Read(barcode);

        layout.PassengerDescription.ToTrimmedString(barcode).Should().Be("1");
        layout.BaggageTagLicensePlate.ToTrimmedString(barcode).Should().Be("0014123456003");

        var first = layout.Legs[0];
        first.OperatingCarrierPnrCode.ToTrimmedString(barcode).Should().Be("ABC123");
        first.DocumentFormSerialNumber.ToTrimmedString(barcode).Should().Be("1234567890");
        first.FreeBaggageAllowance.ToTrimmedString(barcode).Should().Be("20K");
        first.FastTrack.ToChar(barcode).Should().Be('Y');
        first.AirlineIndividualUse.ToTrimmedString(barcode).Should().Be("LX58Z");

        var second = layout.Legs[1];
        second.OperatingCarrierPnrCode.ToTrimmedString(barcode).Should().Be("DEF456");
        second.FromCityAirportCode.ToTrimmedString(barcode).Should().Be("FRA");
        second.ToCityAirportCode.ToTrimmedString(barcode).Should().Be("GVA");
        second.DocumentFormSerialNumber.ToTrimmedString(barcode).Should().Be("0987654321");
        second.FreeBaggageAllowance.ToTrimmedString(barcode).Should().Be("2PC");
        second.FastTrack.ToChar(barcode).Should().Be('N');
        second.AirlineIndividualUse.ToTrimmedString(barcode).Should().Be("WQ");

        layout.SecurityDataType.ToChar(barcode).Should().Be('1');
        layout.SecurityData.ToTrimmedString(barcode).Should().HaveLength(0x64);
        layout.TrailingData.IsPresent.Should().BeFalse();
    }

    [Fact]
    public void Matches_BoardingPassKit_expectations_for_its_multi_leg_pass()
    {
        const string barcode = TestBarcodes.BoardingPassKitMultiLeg;
        var layout = Read(barcode);

        layout.VersionNumber.ToChar(barcode).Should().Be('2');
        layout.SourceOfCheckIn.ToChar(barcode).Should().Be('M');
        layout.AirlineDesignatorOfIssuer.ToTrimmedString(barcode).Should().Be("AS");

        layout.Legs.Select(l => l.FrequentFlyerNumber.ToTrimmedString(barcode)).Should().OnlyContain(n => n == "76UXK84");
        layout.Legs.Select(l => l.SelecteeIndicator.ToChar(barcode)).Should().OnlyContain(c => c == '3');
        layout.Legs[0].AirlineIndividualUse.ToTrimmedString(barcode).Should().Be("1");
        layout.Legs[1].AirlineIndividualUse.ToTrimmedString(barcode).Should().Be("1    01010");

        layout.SecurityDataType.ToChar(barcode).Should().Be('4');
        layout.SecurityData.ToRawString(barcode).Should()
            .Be("MEQCICRNjFGBPfJr84Ma6vMjxTQLtZ1z7uB0tUfO+fS/3vvuAiAReH4kY4ZcmXR+vD8Y+KoA1Dn1YKpr8YxCYbREeOYcsA==");
    }

    [Fact]
    public void Reads_unique_conditional_items_from_the_first_leg_even_when_its_mandatory_items_are_blank()
    {
        const string barcode = TestBarcodes.BcbpEmptyFirstLeg;
        var layout = Read(barcode);

        layout.VersionNumber.ToChar(barcode).Should().Be('6');
        layout.Legs[0].OperatingCarrierPnrCode.ToTrimmedString(barcode).Should().BeNull();
        layout.Legs[1].OperatingCarrierPnrCode.ToTrimmedString(barcode).Should().Be("ABC123");
        layout.Legs[1].RepeatedConditionalsSize.ToRawString(barcode).Should().Be("00");
    }

    [Fact]
    public void Tolerates_stripped_trailing_padding()
    {
        var barcode = TestBarcodes.BoardingPassKitSimple.TrimEnd()[..^3].TrimEnd();
        var layout = Read(barcode);

        layout.Legs[0].FrequentFlyerNumber.ToTrimmedString(barcode).Should().Be("76UXK84");
        layout.Legs[0].IdAdIndicator.IsPresent.Should().BeFalse();
        layout.Legs[0].AirlineIndividualUse.IsPresent.Should().BeFalse();
    }

    [Fact]
    public void Keeps_unstructured_variable_size_data_as_airline_use()
    {
        const string barcode = TestBarcodes.MichelGeorgeMalformed;
        var layout = Read(barcode);

        layout.VersionNumber.IsPresent.Should().BeFalse();
        layout.Legs[0].AirlineIndividualUse.ToRawString(barcode).Should().Be("3");
        layout.TrailingData.ToRawString(barcode).Should().Be(" 147>218  W    B                29");
    }

    [Theory]
    [InlineData(TestBarcodes.Version8Unspecified, 'X')]
    [InlineData(TestBarcodes.Version8Undisclosed, 'U')]
    public void Reads_version_8_passenger_descriptions(string barcode, char expected)
    {
        var layout = Read(barcode);

        layout.VersionNumber.ToChar(barcode).Should().Be('8');
        layout.PassengerDescription.ToChar(barcode).Should().Be(expected);
    }
}
