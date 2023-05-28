using System;

namespace IATA.BarCodedBoardingPass;

public class IataParsingException : Exception
{
    public IataParsingException(string barcode, object data, Exception innerException) : 
        base($"Unable to parse the IATA barcode \"{barcode}\"", innerException)
    {
        Data.Add("Barcode", barcode);
        Data.Add("Data", data);
    }
}