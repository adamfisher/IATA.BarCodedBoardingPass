namespace IATA.BarCodedBoardingPass.Test;

public class BoardingPassTests
{
    private const string BcbpSecurityData =
        "GIWVC5EH7JNT684FVNJ91W2QA4DVN5J8K4F0L0GEQ3DF5TGBN8709HKT5D3DW3GBHFCVHMY7J5T6HFR41W2QA4DVN5J8K4F0L0GE";

    [Fact]
    public void Parses_a_multi_leg_pass_with_security_data()
    {
        var pass = BoardingPass.Parse(TestBarcodes.BcbpFull, new DateOnly(2016, 12, 31));

        pass.Should().BeEquivalentTo(new BoardingPass
        {
            FormatCode = 'M',
            PassengerName = "DESMARAIS/LUC",
            ElectronicTicketIndicator = 'E',
            Version = 6,
            PassengerDescription = PassengerDescription.Male,
            CheckInSource = CheckInSource.Web,
            IssuanceSource = BoardingPassIssuanceSource.Web,
            IssueDate = new DateOnly(2016, 8, 12),
            DocumentType = DocumentType.BoardingPass,
            IssuingAirline = "AC",
            BaggageTagNumber = "0014123456003",
            Security = new SecurityData('1', BcbpSecurityData),
            Legs =
            [
                new FlightLeg
                {
                    OperatingCarrierPnrCode = "ABC123",
                    FromAirport = "YUL",
                    ToAirport = "FRA",
                    OperatingCarrier = "AC",
                    FlightNumber = "0834",
                    FlightDate = new DateOnly(2016, 8, 13),
                    CompartmentCode = 'F',
                    SeatNumber = "001A",
                    CheckInSequenceNumber = "0025",
                    PassengerStatus = '1',
                    AirlineNumericCode = "014",
                    DocumentFormSerialNumber = "1234567890",
                    InternationalDocumentationVerification = InternationalDocumentationVerification.Required,
                    MarketingCarrier = "AC",
                    FrequentFlyerAirline = "AC",
                    FrequentFlyerNumber = "1234567890123",
                    FreeBaggageAllowance = "20K",
                    FastTrack = true,
                    AirlineIndividualUse = "LX58Z",
                },
                new FlightLeg
                {
                    OperatingCarrierPnrCode = "DEF456",
                    FromAirport = "FRA",
                    ToAirport = "GVA",
                    OperatingCarrier = "LH",
                    FlightNumber = "3664",
                    FlightDate = new DateOnly(2016, 8, 14),
                    CompartmentCode = 'C',
                    SeatNumber = "012C",
                    CheckInSequenceNumber = "0002",
                    PassengerStatus = '1',
                    AirlineNumericCode = "014",
                    DocumentFormSerialNumber = "0987654321",
                    InternationalDocumentationVerification = InternationalDocumentationVerification.Required,
                    MarketingCarrier = "AC",
                    FrequentFlyerAirline = "AC",
                    FrequentFlyerNumber = "1234567890123",
                    FreeBaggageAllowance = "2PC",
                    FastTrack = false,
                    AirlineIndividualUse = "WQ",
                },
            ],
        });
    }

    [Theory]
    [InlineData(TestBarcodes.Version8Unspecified, PassengerDescription.Unspecified)]
    [InlineData(TestBarcodes.Version8Undisclosed, PassengerDescription.Undisclosed)]
    public void Parses_version_8_gender_codes(string barcode, PassengerDescription expected)
    {
        var pass = BoardingPass.Parse(barcode, new DateOnly(2026, 10, 12));

        pass.Version.Should().Be(8);
        pass.PassengerDescription.Should().Be(expected);
    }

    [Fact]
    public void Parses_every_version_8_item()
    {
        var pass = BoardingPass.Parse(TestBarcodes.Version8Undisclosed, new DateOnly(2026, 10, 12));

        pass.LastName.Should().Be("SMITH");
        pass.FirstName.Should().Be("TAYLOR");
        pass.CheckInSource.Should().Be(CheckInSource.Web);
        pass.IssuanceSource.Should().Be(BoardingPassIssuanceSource.Web);
        pass.IssueDate.Should().Be(new DateOnly(2026, 10, 7));
        pass.DocumentType.Should().Be(DocumentType.BoardingPass);
        pass.IssuingAirline.Should().Be("DL");
        pass.Security.Should().Be(new SecurityData('1', "MEUCIQDSIGNATURE"));
        pass.TrailingData.Should().BeNull();

        var leg = pass.Legs.Should().ContainSingle().Subject;
        leg.FlightDate.Should().Be(new DateOnly(2026, 10, 8));
        leg.InternationalDocumentationVerification.Should().Be(InternationalDocumentationVerification.Required);
        leg.FrequentFlyerNumber.Should().Be("1234567890");
        leg.IdAdIndicator.Should().Be('0');
        leg.FreeBaggageAllowance.Should().Be("1PC");
        leg.FastTrack.Should().BeTrue();
        leg.AirlineIndividualUse.Should().Be("XYZ");
    }

    [Fact]
    public void Matches_BoardingPassKit_expectations()
    {
        var pass = BoardingPass.Parse(TestBarcodes.BoardingPassKitInternational, new DateOnly(2019, 12, 31));

        pass.PassengerName.Should().Be("ACKERMANN/JUSTIN DAV");
        pass.Version.Should().Be(3);
        pass.PassengerDescription.Should().Be(PassengerDescription.Adult);
        pass.CheckInSource.Should().BeNull();
        pass.IssuanceSource.Should().Be(BoardingPassIssuanceSource.AirportAgent);
        pass.IssueDate.Should().Be(new DateOnly(2019, 12, 1));
        pass.BaggageTagNumber.Should().Be("0131536170001");

        var leg = pass.Legs.Should().ContainSingle().Subject;
        leg.OperatingCarrierPnrCode.Should().Be("JPYKJI");
        leg.FlightNumber.Should().Be("0712");
        leg.FlightDate.Should().Be(new DateOnly(2019, 12, 2));
        leg.SeatNumber.Should().Be("025C");
        leg.CheckInSequenceNumber.Should().Be("0231");
        leg.AirlineNumericCode.Should().Be("001");
        leg.DocumentFormSerialNumber.Should().Be("7460111872");
        leg.SelecteeIndicator.Should().Be('0');
        leg.MarketingCarrier.Should().Be("JL");
        leg.FrequentFlyerAirline.Should().Be("AA");
        leg.FrequentFlyerNumber.Should().Be("76UXK84");
        leg.AirlineIndividualUse.Should().Be("3");
    }

    [Fact]
    public void Keeps_codes_this_library_does_not_name()
    {
        var barcode = TestBarcodes.Version8Unspecified.Replace(">808X", ">8089", StringComparison.Ordinal);

        var pass = BoardingPass.Parse(barcode, new DateOnly(2026, 10, 12));

        pass.PassengerDescription.Should().Be((PassengerDescription)'9');
        pass.Encode().Should().Be(barcode);
    }

    [Theory]
    [InlineData(TestBarcodes.BcbpRandom, 2016, "2016-08-12", "2016-08-13")]
    [InlineData("M1DESMARAIS/LUC       EABC123    FRAAC      226F001A      3B>60B1W 0225BAC 2A   1234567890 1AC AC 1234567890123    20KY", 2010, "2010-08-13", "2010-08-14")]
    [InlineData(TestBarcodes.BcbpRandom, 2006, "2006-08-13", "2006-08-14")]
    [InlineData(TestBarcodes.BcbpFlightDateBeforeIssue, 2023, "2021-12-30", "2022-01-01")]
    public void Works_out_the_year_of_dates_from_the_issue_date(string barcode, int referenceYear, string issueDate, string flightDate)
    {
        var pass = BoardingPass.Parse(barcode, new DateOnly(referenceYear, 12, 31));

        pass.IssueDate.Should().Be(DateOnly.Parse(issueDate, System.Globalization.CultureInfo.InvariantCulture));
        pass.Legs[0].FlightDate.Should().Be(DateOnly.Parse(flightDate, System.Globalization.CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData("2026-01-02", "365", "2025-12-31")]
    [InlineData("2025-12-30", "002", "2026-01-02")]
    [InlineData("2026-06-01", "226", "2026-08-14")]
    public void Without_an_issue_date_uses_the_flight_date_nearest_the_reference_date(string reference, string dayOfYear, string expected)
    {
        var barcode = TestBarcodes.BcbpBasic.Replace(" 226F", $" {dayOfYear}F", StringComparison.Ordinal);

        var pass = BoardingPass.Parse(barcode, DateOnly.Parse(reference, System.Globalization.CultureInfo.InvariantCulture));

        pass.Legs[0].FlightDate.Should().Be(DateOnly.Parse(expected, System.Globalization.CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData(TestBarcodes.DoeJohn)]
    [InlineData(TestBarcodes.BcbpBasic)]
    [InlineData(TestBarcodes.BcbpRandom)]
    [InlineData(TestBarcodes.BcbpFull)]
    [InlineData(TestBarcodes.BcbpEmptyFirstLeg)]
    [InlineData(TestBarcodes.BcbpFlightDateBeforeIssue)]
    [InlineData(TestBarcodes.Version8Unspecified)]
    [InlineData(TestBarcodes.Version8Undisclosed)]
    public void Encode_round_trips(string barcode)
    {
        BoardingPass.Parse(barcode, new DateOnly(2026, 10, 12)).Encode().Should().Be(barcode);
    }

    [Theory]
    [MemberData(nameof(TestBarcodes.All), MemberType = typeof(TestBarcodes))]
    public void Encoded_passes_parse_back_to_the_same_values(string barcode)
    {
        var reference = new DateOnly(2026, 10, 12);
        var pass = BoardingPass.Parse(barcode, reference) with { TrailingData = null };

        BoardingPass.Parse(pass.Encode(), reference).Should().BeEquivalentTo(pass);
    }

    [Fact]
    public void Encodes_a_new_pass()
    {
        var pass = new BoardingPass
        {
            PassengerName = "DESMARAIS/LUC",
            Legs =
            [
                new FlightLeg
                {
                    OperatingCarrierPnrCode = "ABC123",
                    FromAirport = "YUL",
                    ToAirport = "FRA",
                    OperatingCarrier = "AC",
                    FlightNumber = "0834",
                    FlightDate = new DateOnly(2020, 8, 13),
                    CompartmentCode = 'F',
                    SeatNumber = "001A",
                    CheckInSequenceNumber = "0025",
                    PassengerStatus = '1',
                },
            ],
        };

        pass.Encode().Should().Be("M1DESMARAIS/LUC       EABC123 YULFRAAC 0834 226F001A0025 100");
        (pass with { Version = 6 }).Encode().Should().Be(TestBarcodes.BcbpBasic);
        (pass with { PassengerDescription = PassengerDescription.Undisclosed }).Encode()
            .Should().Be("M1DESMARAIS/LUC       EABC123 YULFRAAC 0834 226F001A0025 107>801U00");
    }

    [Fact]
    public void Changes_are_made_with_with_expressions()
    {
        var pass = BoardingPass.Parse(TestBarcodes.BcbpBasic);

        var moved = pass with { Legs = [pass.Legs[0] with { SeatNumber = "014C" }] };

        moved.Encode().Should().Be(TestBarcodes.BcbpBasic.Replace("001A", "014C", StringComparison.Ordinal));
        pass.Legs[0].SeatNumber.Should().Be("001A");
    }

    [Fact]
    public void TryParse_returns_false_instead_of_throwing()
    {
        BoardingPass.TryParse("not a boarding pass", out var pass).Should().BeFalse();
        pass.Should().BeNull();
        BoardingPass.TryParse((string?)null, out _).Should().BeFalse();
        BoardingPass.TryParse(TestBarcodes.DoeJohn, out pass).Should().BeTrue();
        pass!.LastName.Should().Be("DOE");
    }

    [Fact]
    public void Parse_reports_where_the_barcode_is_wrong()
    {
        var parse = () => BoardingPass.Parse("M1DOE/JOHN            EXYZ123 ZRHSFOBA 1234 147F035A0001 1ZZ");

        parse.Should().Throw<BarcodedBoardingPassFormatException>().Which.Position.Should().Be(58);
    }

    [Fact]
    public void Works_with_generic_parsing()
    {
        static T ParseAny<T>(string s) where T : ISpanParsable<T> => T.Parse(s.AsSpan(), null);

        var pass = ParseAny<BoardingPass>(TestBarcodes.BcbpBasic);

        pass.Encode().Should().Be(TestBarcodes.BcbpBasic);
        $"{pass}".Should().StartWith("BoardingPass {", "records keep their debugging ToString()");
    }

    [Theory]
    [InlineData(TestBarcodes.BcbpBasic)]
    [InlineData(TestBarcodes.BcbpFull)]
    [InlineData(TestBarcodes.Version8Undisclosed)]
    public void TryFormat_succeeds_only_when_the_destination_is_large_enough(string barcode)
    {
        var pass = BoardingPass.Parse(barcode, new DateOnly(2026, 10, 12));

        for (var size = 0; size <= barcode.Length + 1; size++)
        {
            var destination = new char[size];
            var fits = size >= barcode.Length;

            pass.TryFormat(destination, out var written).Should().Be(fits, $"the destination holds {size} characters");

            if (fits)
            {
                new string(destination, 0, written).Should().Be(barcode);
            }
        }
    }

    [Fact]
    public void Encode_rejects_passes_that_cannot_be_encoded()
    {
        var noLegs = () => new BoardingPass().Encode();
        var tooLong = () => (BoardingPass.Parse(TestBarcodes.BcbpBasic) with
        {
            Legs = [new FlightLeg { AirlineIndividualUse = new string('A', 300) }],
        }).Encode();

        noLegs.Should().Throw<InvalidOperationException>();
        tooLong.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Exposes_data_that_is_not_part_of_the_structure()
    {
        var pass = BoardingPass.Parse(TestBarcodes.MichelGeorgeMalformed);

        pass.Version.Should().BeNull();
        pass.Legs[0].AirlineIndividualUse.Should().Be("3");
        pass.TrailingData.Should().Be(" 147>218  W    B                29");
    }
}
