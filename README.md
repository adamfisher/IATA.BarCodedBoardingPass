#  [![](https://raw.githubusercontent.com/adamfisher/IATA.BarCodedBoardingPass/main/IATA.BarCodedBoardingPass/iata_logo.png)](#) IATA.BarCodedBoardingPass

[![Nuget](https://img.shields.io/nuget/dt/IATA.BarCodedBoardingPass?color=blue&label=nuget&style=plastic)](https://www.nuget.org/packages/IATA.BarCodedBoardingPass)
[![CI](https://github.com/adamfisher/IATA.BarCodedBoardingPass/actions/workflows/ci.yml/badge.svg)](https://github.com/adamfisher/IATA.BarCodedBoardingPass/actions/workflows/ci.yml)

A .NET library that parses and encodes International Air Transport Association (IATA) Bar Coded Boarding Passes (BCBP): the text inside the PDF417, Aztec, QR or Data Matrix barcode on a boarding pass.

- Implements [IATA Resolution 792](https://www.iata.org/en/programs/passenger/common-use/) up to **version 8**, including the version 8 passenger descriptions `X` (unspecified) and `U` (undisclosed)
- Reads every leg of multi-leg passes, conditional items, airline data and security data
- Tolerates the trailing spaces that scanners and copy/paste often strip
- Encodes passes back to BCBP text
- Targets .NET 10

## Install

```
dotnet add package IATA.BarCodedBoardingPass
```

## Parse

```csharp
using IATA.BarCodedBoardingPass;

var pass = BoardingPass.Parse("M1DESMARAIS/LUC       EABC123 YULFRAAC 0834 226F001A0025 106>60000");

var leg = pass.Legs[0];
Console.WriteLine($"{pass.LastName}, {pass.FirstName}");                     // DESMARAIS, LUC
Console.WriteLine($"{leg.OperatingCarrier}{leg.FlightNumber} {leg.FromAirport}-{leg.ToAirport}"); // AC0834 YUL-FRA
Console.WriteLine($"{leg.FlightDate:yyyy-MM-dd} seat {leg.SeatNumber}");    // e.g. 2026-08-14 seat 001A
```

`TryParse` returns `false` instead of throwing, and both methods accept a `string` or a `ReadOnlySpan<char>`:

```csharp
if (BoardingPass.TryParse(scannedText, out var pass))
{
    // ...
}
```

`Parse` throws a `BarcodedBoardingPassFormatException` whose `Position` says where the text stopped looking like a boarding pass.

### What you get

| Member | Type | Notes |
|---|---|---|
| `PassengerName`, `LastName`, `FirstName` | `string?` | |
| `Legs` | `IReadOnlyList<FlightLeg>` | PNR, airports, carrier, flight number, `FlightDate`, compartment, seat, check-in sequence, frequent flyer, baggage allowance, `FastTrack`, airline data… |
| `PassengerDescription` | `PassengerDescription?` | `Adult`, `Male`, `Female`, `Child`, `Infant`, …, `Unspecified`, `Undisclosed` |
| `CheckInSource`, `IssuanceSource`, `DocumentType` | enums | |
| `IssueDate` | `DateOnly?` | |
| `BaggageTagNumber` and the two non-consecutive tags | `string?` | |
| `Version` | `int?` | Version of the standard the pass was encoded with |
| `Security` | `SecurityData?` | Type and data, e.g. a digital signature |

Text values have their trailing padding removed and are otherwise unchanged (`"001A"`, `"0834"`). Values that are blank or not encoded are `null`. Enum values are the characters used in the barcode, so a code the library doesn't name is kept: `(PassengerDescription)'9'`.

### Dates

The barcode stores only the day of the year for the flight date, and only the last digit of the year for the issue date. The year is worked out like this:

- **Issue date:** the latest matching date that is not after the reference date.
- **Flight date:** the first matching day on or after the issue date. If the pass has no issue date, the matching day nearest the reference date.

The reference date defaults to today (UTC). Pass the scan date when parsing older passes:

```csharp
var pass = BoardingPass.Parse(text, referenceDate: new DateOnly(2016, 12, 31));
```

## Encode

Boarding passes are immutable records. Use `with` to change a pass and `Encode()` to turn it back into BCBP text:

```csharp
var moved = pass with { Legs = [pass.Legs[0] with { SeatNumber = "014C" }] };
string barcode = moved.Encode();
```

Or build one from scratch:

```csharp
var pass = new BoardingPass
{
    PassengerName = "DESMARAIS/LUC",
    PassengerDescription = PassengerDescription.Undisclosed,
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

pass.Encode(); // M1DESMARAIS/LUC       EABC123 YULFRAAC 0834 226F001A0025 107>801U00
```

Conditional items without a value at the end of a section are left out. A pass that has conditional items but no `Version` is encoded as version 8. `TryFormat(Span<char>, out int)` encodes without allocating.

## `IataParser` (original API)

The original dictionary and `IataData` API is still available. It now uses the same parser, so it reads long, multi-leg and version 8 passes correctly. It still returns only the first leg, with values padded as encoded:

```csharp
var result = IataParser.Decode(barcode);
var passengerName = result["Passenger Name"];   // "DESMARAIS/LUC       "

var data = IataParser.DecodeObject(barcode);    // trimmed values
```

A field that is present but blank is `"(empty field)"`. A field that is not in the barcode is an empty string.

## Standards

- [IATA Resolution 792](https://www.iata.org/en/programs/passenger/common-use/) version 8 took effect on 1 June 2020. Its only change from version 7 is the passenger description codes `X` and `U`.
- [BCBP Implementation Guide, version 7 (PDF)](https://www.iata.org/contentassets/1dccc9ed041b4f3bbdcf8ee8682e75c4/2021_03_02-bcbp-implementation-guide-version-7-.pdf)

## Acknowledgements

Originally inspired by [georgesmith46/bcbp](https://github.com/georgesmith46/bcbp) and ported from [menepet/iata-barcode-js-parser](https://github.com/menepet/iata-barcode-js-parser). The tests include sample passes from [bcbp](https://github.com/georgesmith46/bcbp) and [BoardingPassKit](https://github.com/anomaddev/BoardingPassKit), both MIT licensed.

## Building

Requires the .NET 10 SDK.

```
dotnet build
dotnet test
dotnet pack IATA.BarCodedBoardingPass -c Release
```
