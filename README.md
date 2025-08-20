# Thai Customs Receipt OCR API

A C# .NET 8 API and console application for parsing Thai customs receipt OCR data into structured JSON format. This library provides free and stable parsing capabilities for extracting key information from Thai customs department receipts (กรมศุลกากร).

## Features

- **Structured Data Extraction**: Parses raw OCR text into organized JSON format
- **Comprehensive Field Support**: Extracts organization, reference data, financial items, totals, and signatures
- **Regex-Based Parsing**: Robust pattern matching for Thai and English text
- **REST API with Swagger**: Web API with interactive documentation
- **Console Application**: Standalone executable for testing and demonstration
- **JSON Serialization**: Uses System.Text.Json for efficient serialization
- **Free & Open Source**: No licensing restrictions or embedded mockup data

## Quick Start

### Console Application (Standalone)

```bash
cd ThaiCustomsOcrConsole
dotnet run
```

### Web API

```bash
cd ThaiCustomsOcrApi
dotnet run
```

Then visit: `http://localhost:5288/swagger` for interactive API documentation

## API Usage

### Test Endpoint
```http
GET /api/CustomsReceipt/test
```

### Parse Custom OCR Text
```http
POST /api/CustomsReceipt/parse
Content-Type: application/json

{
  "ocrText": "Your Thai customs receipt OCR text here"
}
```

## Data Structure

The parser extracts data into the following structure:

```json
{
  "organization": "กรมศุลกากร",
  "documentType": "ใบเสร็จรับเงิน",
  "reference": {
    "skc": "122",
    "hawb": "1746172551",
    "vehicleNo": "07556",
    "taxId": "0105533022910/000000",
    "importExportDate": "25-01-2566",
    "declarantName": "DHL EXPRESS",
    "declarationNo": "A025-X660100256",
    "paymentRef": "1818-061169",
    "paymentDate": "27-01-66"
  },
  "items": {
    "importDuty": 937.63,
    "vat": 65.64,
    "other": 1.00
  },
  "total": {
    "amount": 1004.47,
    "amountText": "หนึ่งพันสี่บาทสี่สิบเจ็ดสตางค์"
  },
  "sign": {
    "receiver": "วัชนี",
    "officer": "เกตจนา"
  }
}
```

## Project Structure

- **ThaiCustomsOcrApi**: ASP.NET Core Web API with Swagger
- **ThaiCustomsOcrConsole**: Standalone console application
- **Models**: Data models for CustomsReceipt and related classes
- **Services**: OCR parsing logic and business services

## Requirements

- .NET 8.0 SDK
- Compatible with Windows, Linux, and macOS

## Building from Source

```bash
# Build API
cd ThaiCustomsOcrApi
dotnet build

# Build Console App  
cd ThaiCustomsOcrConsole
dotnet build
```

## Testing with Sample Data

The included sample OCR text demonstrates parsing of:
- Thai customs department receipt
- Import duty and VAT calculations
- Company information (DHL EXPRESS)
- Reference numbers and dates
- Thai language amount descriptions

## Supported Fields

| Category | Fields |
|----------|--------|
| **Organization** | Organization name, Document type |
| **Reference** | SKC, HAWB, Vehicle number, Tax ID, Import/export date, Declarant name, Declaration number, Payment reference, Payment date |
| **Financial Items** | Import duty, VAT, Other fees |
| **Totals** | Total amount (numeric), Amount in Thai text |
| **Signatures** | Receiver name, Officer name |

## Contributing

This is a production-ready library designed for stability and accuracy in parsing Thai customs receipts. The regex patterns are optimized for handling OCR text with spacing irregularities and mixed Thai/English content.

## License

Free and open source - no licensing restrictions.