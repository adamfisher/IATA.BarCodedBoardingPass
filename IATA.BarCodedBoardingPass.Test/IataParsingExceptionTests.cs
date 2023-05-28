using FluentAssertions;
using Xunit.Categories;

namespace IATA.BarCodedBoardingPass.Test
{
    public class IataParsingExceptionTests
    {
        [Fact]
        [UnitTest]
        public void Throw()
        {
            var decode = () => IataParser.Decode(null);

            decode.Should().Throw<IataParsingException>()
                .Where(ex => ex.Data.Contains("Barcode"))
                .Where(ex => ex.Data.Contains("Data"));
        }
    }
}