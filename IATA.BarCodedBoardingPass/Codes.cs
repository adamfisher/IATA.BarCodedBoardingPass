namespace IATA.BarCodedBoardingPass;

// Each enum's values are the characters used in the barcode, so a code this library does not
// name is still preserved: (PassengerDescription)'9' round trips unchanged.

/// <summary>Passenger Description (item 15).</summary>
public enum PassengerDescription : ushort
{
    /// <summary><c>0</c></summary>
    Adult = '0',

    /// <summary><c>1</c></summary>
    Male = '1',

    /// <summary><c>2</c></summary>
    Female = '2',

    /// <summary><c>3</c></summary>
    Child = '3',

    /// <summary><c>4</c></summary>
    Infant = '4',

    /// <summary><c>5</c>, a seat for cabin baggage or similar.</summary>
    NoPassenger = '5',

    /// <summary><c>6</c></summary>
    AdultTravellingWithInfant = '6',

    /// <summary><c>7</c></summary>
    UnaccompaniedMinor = '7',

    /// <summary><c>X</c>, added in version 8.</summary>
    Unspecified = 'X',

    /// <summary><c>U</c>, added in version 8.</summary>
    Undisclosed = 'U',
}

/// <summary>Source of check-in (item 12).</summary>
public enum CheckInSource : ushort
{
    /// <summary><c>W</c></summary>
    Web = 'W',

    /// <summary><c>K</c></summary>
    AirportKiosk = 'K',

    /// <summary><c>R</c></summary>
    RemoteOrOffSiteKiosk = 'R',

    /// <summary><c>M</c></summary>
    MobileDevice = 'M',

    /// <summary><c>O</c></summary>
    AirportAgent = 'O',

    /// <summary><c>T</c></summary>
    TownAgent = 'T',

    /// <summary><c>V</c></summary>
    ThirdPartyVendor = 'V',
}

/// <summary>Source of Boarding Pass Issuance (item 14).</summary>
public enum BoardingPassIssuanceSource : ushort
{
    /// <summary><c>W</c></summary>
    Web = 'W',

    /// <summary><c>K</c></summary>
    AirportKiosk = 'K',

    /// <summary><c>X</c></summary>
    TransferKiosk = 'X',

    /// <summary><c>R</c></summary>
    RemoteOrOffSiteKiosk = 'R',

    /// <summary><c>M</c></summary>
    MobileDevice = 'M',

    /// <summary><c>O</c></summary>
    AirportAgent = 'O',

    /// <summary><c>T</c></summary>
    TownAgent = 'T',

    /// <summary><c>V</c></summary>
    ThirdPartyVendor = 'V',
}

/// <summary>Document Type (item 16).</summary>
public enum DocumentType : ushort
{
    /// <summary><c>B</c></summary>
    BoardingPass = 'B',

    /// <summary><c>I</c></summary>
    ItineraryReceipt = 'I',
}

/// <summary>International Documentation Verification (item 108).</summary>
public enum InternationalDocumentationVerification : ushort
{
    /// <summary><c>0</c></summary>
    NotRequired = '0',

    /// <summary><c>1</c></summary>
    Required = '1',

    /// <summary><c>2</c></summary>
    Performed = '2',
}
