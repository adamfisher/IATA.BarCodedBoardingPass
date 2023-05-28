# IATA.BarCodedBoardingPass

A C# parser for International Air Transport Association (IATA) barcodes from a passenger boarding pass.

**Ported from [menepet/iata-barcode-js-parser](https://github.com/menepet/iata-barcode-js-parser).**

The security field (after 158 chars barcode length) is a separate field that enables a third party to verify that the bar code data was not tampered with. The security field is optional and to be used only when required by the local security administration. Typically, this field may contain a digital signature of variable length, the length of the field and a type of security data (that defines the algorithm used). IATA is only providing the structure for the signature to be stored in the bar code. The bar code data (mandatory, optional and individual airline use fields) remain unchanged and can be read regardless of the digital signature.

## Usage

You can decode barcodes into a dictionary or a strongly typed object type called `IataData`:

```csharp
var barcodeData = "M1MICHEL/GEORGE       E6C2KLS ATHTORAC 1903 0114 185Y009A0013 147>218  W    B                29";

// Get data in dictionary format
var result = IataParser.Decode(barcodeData);
var passengerName = result["Passenger Name"];
// ... and so on...

// Get data in object format
var data = IataParser.DecodeObject(barcodeData);

/*
Data object containing fields like:
{
	BaggageTagLicensePlate = " B ",
	CheckInSequenceNumber = "Y009A",
	CompartmentCode = "4",
	ConditionalsSize = "01",
	DateOfFlight = "011",
	DateOfPassIssuance = "18 ",
	DocumentType = "W",
	ElectronicTicketIndicator = "E",
	FlightNumber = "1903 ",
	FormatCode = "M",
	FromCityAirportCode = "ATH",
	OperatingCarrierDesignator = "AC ",
	OperatingCarrierPnrCode = "6C2KLS ",
	PassengerDescription = "7",
	PassengerName = "MICHEL/GEORGE ",
	PassengerStatus = "0",
	SeatNumber = " 185",
	SourceOfBoardingPassIssuance = "2",
	SourceOfCheckIn = ">",
	ToCityAirportCode = "TOR",
	UniqueConditionalsSize = "14"
}
*/
```
