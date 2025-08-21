# OCR PDF and Images API

A FastAPI-based web service that provides OCR (Optical Character Recognition) capabilities for PDF files and images. The API accepts file uploads and returns extracted text using Tesseract OCR engine.

## Features

- **File Upload Support**: Accepts PDF and common image formats (JPG, PNG, TIFF, BMP, WebP)
- **OCR Processing**: Uses Tesseract OCR for text extraction
- **Structured Responses**: Returns both raw text and structured objects with metadata
- **Error Handling**: Comprehensive validation and error reporting
- **Health Monitoring**: Built-in health check endpoint

## Installation

### Prerequisites

- Python 3.8+
- Tesseract OCR
- Poppler (for PDF processing)

### Install System Dependencies

**Ubuntu/Debian:**
```bash
sudo apt-get update
sudo apt-get install -y tesseract-ocr tesseract-ocr-eng poppler-utils
```

**macOS:**
```bash
brew install tesseract poppler
```

### Install Python Dependencies

```bash
pip install -r requirements.txt
```

## Usage

### Starting the Server

```bash
python main.py
```

The server will start on `http://localhost:8000` by default.

### API Endpoints

#### 1. Root Endpoint
```
GET /
```
Returns API information and available endpoints.

**Response:**
```json
{
  "message": "OCR PDF and Images API",
  "version": "1.0.0",
  "endpoints": {
    "upload": "/upload-file/",
    "health": "/health/"
  }
}
```

#### 2. Health Check
```
GET /health/
```
Checks if the service and OCR engine are working properly.

**Response:**
```json
{
  "status": "healthy",
  "ocr_engine": "tesseract"
}
```

#### 3. File Upload and OCR Processing
```
POST /upload-file/
```
Upload a file for OCR processing.

**Request:**
- Method: POST
- Content-Type: multipart/form-data
- Body: Form data with file field

**Supported File Types:**
- Images: JPEG, PNG, TIFF, BMP, WebP
- Documents: PDF

**Example using curl:**
```bash
# Upload an image
curl -X POST "http://localhost:8000/upload-file/" \
     -H "accept: application/json" \
     -H "Content-Type: multipart/form-data" \
     -F "file=@path/to/your/image.jpg"

# Upload a PDF
curl -X POST "http://localhost:8000/upload-file/" \
     -H "accept: application/json" \
     -H "Content-Type: multipart/form-data" \
     -F "file=@path/to/your/document.pdf"
```

**Response for Images:**
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

**Response for PDFs:**
```json
{
  "success": true,
  "message": "File processed successfully",
  "data": {
    "filename": "document.pdf",
    "file_type": "application/pdf",
    "total_pages": 2,
    "pages": [
      {
        "page_number": 1,
        "extracted_text": "Text from page 1",
        "character_count": 17
      },
      {
        "page_number": 2,
        "extracted_text": "Text from page 2",
        "character_count": 17
      }
    ],
    "full_text": "Text from page 1\n\nText from page 2",
    "total_characters": 36
  }
}
```

**Error Response:**
```json
{
  "detail": "Error message describing what went wrong"
}
```

### Interactive API Documentation

Once the server is running, you can access interactive API documentation at:

- **Swagger UI**: `http://localhost:8000/docs`
- **ReDoc**: `http://localhost:8000/redoc`

## Testing

Run the test suite:

```bash
pytest test_main.py -v
```

## Error Handling

The API includes comprehensive error handling for:

- **Empty Files**: Returns 400 error for empty uploads
- **Unsupported File Types**: Validates file types using python-magic
- **OCR Processing Errors**: Handles Tesseract OCR failures gracefully
- **PDF Processing Errors**: Manages PDF conversion and processing issues

## Configuration

### Environment Variables

You can configure the server using environment variables:

- `HOST`: Server host (default: 0.0.0.0)
- `PORT`: Server port (default: 8000)

### Tesseract Configuration

The API uses default Tesseract settings. You can modify OCR settings in the `process_image_ocr` function in `main.py`.

## API Response Structure

### Success Response
```json
{
  "success": true,
  "message": "File processed successfully",
  "data": {
    // File-specific data structure
  }
}
```

### Error Response
```json
{
  "detail": "Error description"
}
```

## Development

### Project Structure
```
.
├── main.py              # Main FastAPI application
├── test_main.py         # Test suite
├── requirements.txt     # Python dependencies
└── README.md           # This file
```

### Adding New Features

1. Fork the repository
2. Create a feature branch
3. Add your changes
4. Add tests for new functionality
5. Run the test suite
6. Submit a pull request

## License

[Add your license information here]

## Contributing

[Add contribution guidelines here]