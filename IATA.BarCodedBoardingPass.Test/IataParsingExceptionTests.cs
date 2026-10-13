namespace IATA.BarCodedBoardingPass.Test;

public class IataParsingExceptionTests
{
    [Fact]
    public void Throw()
    {
        var decode = () => IataParser.Decode(null!);

        decode.Should().Throw<IataParsingException>()
            .Where(ex => ex.Data.Contains("Barcode"))
            .Where(ex => ex.Data.Contains("Data"));
    }

    [Theory]
    [InlineData("M", 0)]
    [InlineData("MX", 1)]
    [InlineData("M1DOE/JOHN            EXYZ123 ZRHSFOBA 1234 147F035A0001 1ZZ", 58)]
    [InlineData("M1DOE/JOHN            EXYZ123 ZRHSFOBA 1234 147F035A0001 104>6G0", 62)]
    public void Throws_with_the_position_of_the_problem(string barcode, int position)
    {
        var decode = () => IataParser.Decode(barcode);

        decode.Should().Throw<IataParsingException>()
            .WithInnerException<BarcodedBoardingPassFormatException>()
            .Which.Position.Should().Be(position);
    }
}
