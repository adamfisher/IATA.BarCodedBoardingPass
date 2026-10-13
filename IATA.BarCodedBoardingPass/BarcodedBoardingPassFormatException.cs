namespace IATA.BarCodedBoardingPass;

/// <summary>
/// Thrown when a string does not have the structure of an IATA Bar Coded Boarding Pass.
/// </summary>
public sealed class BarcodedBoardingPassFormatException : FormatException
{
    /// <summary>Creates the exception.</summary>
    /// <param name="message">What was wrong with the barcode.</param>
    /// <param name="position">Zero-based index in the barcode where the problem was found.</param>
    public BarcodedBoardingPassFormatException(string message, int position)
        : base($"{message} (position {position})")
    {
        Position = position;
    }

    /// <summary>Zero-based index in the barcode where the problem was found.</summary>
    public int Position { get; }
}
