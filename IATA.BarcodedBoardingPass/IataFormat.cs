namespace IATA.BarCodedBoardingPass;

internal class IataFormat
{
    public int Length { get; set; }
    public int Offset { get; set; }
    public string Content { get; set; }
    public string Explanation { get; set; }
}