namespace IATA.BarCodedBoardingPass;

/// <summary>
/// Thrown by <see cref="IataParser"/> when a barcode cannot be decoded. <see cref="Exception.Data"/>
/// holds the barcode under <c>"Barcode"</c> and the fields decoded so far under <c>"Data"</c>.
/// </summary>
public class IataParsingException : Exception
{
    /// <summary>Creates the exception.</summary>
    /// <param name="barcode">The barcode that failed to decode.</param>
    /// <param name="data">The fields decoded before the failure.</param>
    /// <param name="innerException">The underlying error.</param>
    public IataParsingException(string barcode, object data, Exception innerException) :
        base($"Unable to parse the IATA barcode \"{barcode}\"", innerException)
    {
        Data.Add("Barcode", barcode);
        Data.Add("Data", data);
    }
}
