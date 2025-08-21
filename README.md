# OCR PDF and Images API (.NET)

A .NET 8 Web API with Swagger for uploading PDF or image files and extracting text using OCR (Optical Character Recognition). This is the .NET implementation that converts the Python FastAPI solution to use .NET + Swagger while maintaining the same functionality.

## Features

- **File Upload Support**: Accepts common image formats (JPG, PNG, TIFF, BMP, WebP)
- **OCR Processing**: Uses Tesseract OCR for robust text extraction
- **Structured Responses**: Returns both raw text and organized metadata
- **Interactive Swagger Documentation**: Auto-generated API documentation
- **Health Monitoring**: Built-in health check endpoint
- **Console Logging**: Prints JSON output to console as requested
- **Free & Stable**: Uses free and stable .NET libraries

## Requirements

- .NET 8.0 SDK
- Tesseract OCR engine
- Compatible with Linux, Windows, and macOS

## Installation

### Prerequisites

**Install Tesseract OCR:**

Ubuntu/Debian:
```bash
sudo apt-get update
sudo apt-get install -y tesseract-ocr tesseract-ocr-eng
```

macOS:
```bash
brew install tesseract
```

Windows:
Download and install from: https://github.com/UB-Mannheim/tesseract/wiki

### Build and Run

```bash
cd OcrApi
dotnet restore
dotnet build
dotnet run
```

The API will start on `http://localhost:5000` (HTTP) and `https://localhost:5001` (HTTPS).

## API Documentation

Once running, access the interactive Swagger documentation at:
- **Swagger UI**: `http://localhost:5000/swagger`

## API Endpoints

### 1. Root Information
```
GET /
```
Returns API information and available endpoints.

**Response:**
```json
{
  "message": "OCR PDF and Images API (.NET Version)",
  "version": "1.0.0",
  "endpoints": {
    "upload": "/api/ocr/upload-file",
    "health": "/api/ocr/health",
    "swagger": "/swagger"
  }
}
```

### 2. Health Check
```
GET /api/ocr/health
```
Checks if the OCR service is working properly.

**Response:**
```json
{
  "status": "healthy",
  "ocr_engine": "Tesseract 4.1.1"
}
```

### 3. File Upload and OCR Processing
```
POST /api/ocr/upload-file
```
Upload an image file for OCR processing.

**Request:**
- Method: POST
- Content-Type: multipart/form-data
- Body: Form data with file field

**Supported File Types:**
- Images: JPEG, PNG, TIFF, BMP, WebP
- PDF: Currently not implemented (will return helpful error message)

**Example using curl:**
```bash
curl -X POST "http://localhost:5000/api/ocr/upload-file" \
     -H "accept: application/json" \
     -H "Content-Type: multipart/form-data" \
     -F "file=@path/to/your/image.jpg"
```

**Success Response:**
```json
{
  "success": true,
  "message": "File processed successfully",
  "data": {
    "filename": "image.jpg",
    "file_type": "image/jpeg",
    "extracted_text": "Text extracted from the image",
    "character_count": 30
  }
}
```

**Error Response:**
```json
{
  "success": false,
  "message": "Error description"
}
```

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **OCR Engine**: Tesseract.NET
- **Image Processing**: SixLabors.ImageSharp
- **API Documentation**: Swagger/OpenAPI
- **JSON Serialization**: System.Text.Json

## Project Structure

```
OcrApi/
├── Controllers/
│   └── OcrController.cs          # API endpoints
├── Models/
│   └── OcrModels.cs              # Data models
├── Services/
│   ├── IOcrService.cs            # Service interface
│   └── TesseractOcrService.cs    # OCR implementation
├── Program.cs                    # Application startup
├── OcrApi.csproj                 # Project file
└── appsettings.json              # Configuration
```

## Configuration

### Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Set to `Development` for Swagger UI
- `ASPNETCORE_URLS`: Configure listening URLs (default: http://localhost:5000)

### Tesseract Configuration

The service automatically searches for Tesseract data files in standard locations:
- `/usr/share/tesseract-ocr/4.00/tessdata`
- `/usr/share/tessdata`
- `/usr/local/share/tessdata`
- `./tessdata`

## Development

### Building
```bash
dotnet build
```

### Running in Development
```bash
dotnet run --environment Development
```

### Testing Endpoints

Use the Swagger UI at `/swagger` or test with curl:

```bash
# Health check
curl http://localhost:5000/api/ocr/health

# API info
curl http://localhost:5000/

# Upload test image (create a simple test image first)
curl -X POST "http://localhost:5000/api/ocr/upload-file" \
     -F "file=@test-image.png"
```

## Differences from Python Version

This .NET implementation provides the same functionality as the Python FastAPI version but with:

- **Better Integration**: Native .NET ecosystem integration
- **Strong Typing**: Compile-time type checking
- **Performance**: Generally better performance characteristics
- **Enterprise Ready**: Production-ready with .NET hosting options
- **Swagger Integration**: Built-in OpenAPI documentation

## Future Enhancements

- PDF processing implementation using .NET PDF libraries
- Multiple language support for OCR
- Image preprocessing options
- Batch processing capabilities
- Authentication and authorization

## License

Free and open source - no licensing restrictions.

## Contributing

This implementation follows the original requirements to be:
- Free and stable
- .NET + Swagger based
- Copy-pasteable and immediately runnable
- Prints JSON to console
- Uses same architectural patterns as previous .NET solution