namespace IATA.BarCodedBoardingPass;

internal sealed class IataFormat
{
    public int Length { get; set; }
    public int Offset { get; set; }
    public required string Content { get; set; }
    public required string Explanation { get; set; }
}
