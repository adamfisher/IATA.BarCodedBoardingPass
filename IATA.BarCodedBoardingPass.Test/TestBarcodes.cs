namespace IATA.BarCodedBoardingPass.Test;

/// <summary>Boarding pass strings shared by the tests.</summary>
public static class TestBarcodes
{
    // Original examples from this repository.

    /// <summary>Single leg, no conditional items.</summary>
    public const string DoeJohn =
        "M1DOE/JOHN            EXYZ123 ZRHSFOBA 1234 147F035A0001 100";

    /// <summary>
    /// Malformed: the mandatory items are shifted by two characters, so the variable size field
    /// is declared as <c>01</c> and the conditional data that follows is not reachable.
    /// </summary>
    public const string MichelGeorgeMalformed =
        "M1MICHEL/GEORGE       E6C2KLS ATHTORAC 1903 0114 185Y009A0013 147>218  W    B                29";

    // From BoardingPassKit testdata/barcodes.json (MIT, https://github.com/anomaddev/BoardingPassKit).

    public const string BoardingPassKitSimple =
        "M1ACKERMANN/JUSTIN DAVEJKLEAJ MSYPHXAA 2819 014S008F0059 14A>318   0014BAA 00000000000002900174844256573 AA AA 76UXK84             223";

    public const string BoardingPassKitHistorical =
        "M1ACKERMANN/JUSTIN    ETDPUPK TPADFWAA 1189 091R003A0033 14A>318   0091BAA 00000000000002900121232782703 AA AA 76UXK84             2IN";

    public const string BoardingPassKitMultiLeg =
        "M2ACKERMANN/JUSTIN DAVEWHFPBW TPASEAAS 0635 213L007A0000 148>2181MM    BAS              25             3    AA 76UXK84         1    WHFPBW SEAJNUAS 0555 213L007A0000 13125             3    AA 76UXK84         1    01010^460MEQCICRNjFGBPfJr84Ma6vMjxTQLtZ1z7uB0tUfO+fS/3vvuAiAReH4kY4ZcmXR+vD8Y+KoA1Dn1YKpr8YxCYbREeOYcsA==";

    public const string BoardingPassKitInternational =
        "M1ACKERMANN/JUSTIN DAVEJPYKJI SINNRTJL 0712 336Y025C0231 348>3180 O9335BJL 01315361700012900174601118720 JL AA 76UXK84             3";

    // From the bcbp package tests (MIT, https://github.com/georgesmith46/bcbp).

    public const string BcbpBasic =
        "M1DESMARAIS/LUC       EABC123 YULFRAAC 0834 226F001A0025 106>60000";

    public const string BcbpRandom =
        "M1DESMARAIS/LUC       EABC123    FRAAC      226F001A      3B>60B1W 6225BAC 2A   1234567890 1AC AC 1234567890123    20KY^164GIWVC5EH7JNT684FVNJ91W2QA4DVN5J8K4F0L0GEQ3DF5TGBN8709HKT5D3DW3GBHFCVHMY7J5T6HFR41W2QA4DVN5J8K4F0L0GE";

    public const string BcbpFull =
        "M2DESMARAIS/LUC       EABC123 YULFRAAC 0834 226F001A0025 14D>6181WW6225BAC 00141234560032A0141234567890 1AC AC 1234567890123    20KYLX58ZDEF456 FRAGVALH 3664 227C012C0002 12E2A0140987654321 1AC AC 1234567890123    2PCNWQ^164GIWVC5EH7JNT684FVNJ91W2QA4DVN5J8K4F0L0GEQ3DF5TGBN8709HKT5D3DW3GBHFCVHMY7J5T6HFR41W2QA4DVN5J8K4F0L0GE";

    public const string BcbpEmptyFirstLeg =
        "M2DESMARAIS/LUC       E                                   06>60000ABC123 YULFRAAC 0834 226F001A0025 10200";

    public const string BcbpFlightDateBeforeIssue =
        "M1DESMARAIS/LUC       EABC123 YULFRAAC 0834 001F001A0025 10D>607   136400";

    // Version 8 (Resolution 792, effective June 2020) gender codes.

    /// <summary>Version 8, passenger description <c>X</c> (unspecified), minimal conditional items.</summary>
    public const string Version8Unspecified =
        "M1DOE/ALEX            EABC123 JFKLAXAA 0100 280Y012A0001 11C>808X  0280B0E00112345678900";

    /// <summary>Version 8, passenger description <c>U</c> (undisclosed), every item, airline data and security data.</summary>
    public const string Version8Undisclosed =
        "M1SMITH/TAYLOR        EQWE123 ATLCDGDL 0082 281J002A0007 13E>80BUWW6280BDL 2A006012345678901DL DL 1234567890      01PCYXYZ^110MEUCIQDSIGNATURE";

    public static TheoryData<string> All =>
    [
        DoeJohn, MichelGeorgeMalformed,
        BoardingPassKitSimple, BoardingPassKitHistorical, BoardingPassKitMultiLeg, BoardingPassKitInternational,
        BcbpBasic, BcbpRandom, BcbpFull, BcbpEmptyFirstLeg, BcbpFlightDateBeforeIssue,
        Version8Unspecified, Version8Undisclosed,
    ];
}
