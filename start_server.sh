#!/bin/bash

# OCR API Server Startup Script
echo "Starting OCR PDF and Images API..."

# Check if Tesseract is installed
if ! command -v tesseract &> /dev/null; then
    echo "Error: Tesseract OCR is not installed."
    echo "Please install it using:"
    echo "  Ubuntu/Debian: sudo apt-get install tesseract-ocr tesseract-ocr-eng poppler-utils"
    echo "  macOS: brew install tesseract poppler"
    exit 1
fi

# Check if Python dependencies are installed
if ! python -c "import fastapi, pytesseract, pdf2image" &> /dev/null; then
    echo "Error: Required Python dependencies are not installed."
    echo "Please install them using: pip install -r requirements.txt"
    exit 1
fi

# Set default host and port if not provided
HOST=${HOST:-"0.0.0.0"}
PORT=${PORT:-8000}

echo "Tesseract version: $(tesseract --version | head -n1)"
echo "Starting server on http://$HOST:$PORT"
echo "API Documentation: http://$HOST:$PORT/docs"
echo "Press Ctrl+C to stop the server"

# Start the server
python main.py