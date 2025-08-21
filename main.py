from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.responses import JSONResponse
import pytesseract
from PIL import Image
from pdf2image import convert_from_bytes
import magic
import io
import tempfile
import os
from typing import Dict, Any, List

app = FastAPI(
    title="OCR PDF and Images API",
    description="API for uploading PDF or image files and extracting text using OCR",
    version="1.0.0"
)

# Supported file types
SUPPORTED_IMAGE_TYPES = {
    'image/jpeg': ['.jpg', '.jpeg'],
    'image/png': ['.png'],
    'image/tiff': ['.tiff', '.tif'],
    'image/bmp': ['.bmp'],
    'image/webp': ['.webp']
}

SUPPORTED_PDF_TYPE = 'application/pdf'

def validate_file_type(file_content: bytes, filename: str) -> str:
    """Validate if the uploaded file is supported."""
    try:
        # Use python-magic to detect actual file type
        mime_type = magic.from_buffer(file_content, mime=True)
        
        if mime_type in SUPPORTED_IMAGE_TYPES:
            return mime_type
        elif mime_type == SUPPORTED_PDF_TYPE:
            return mime_type
        else:
            raise HTTPException(
                status_code=400, 
                detail=f"Unsupported file type: {mime_type}. Supported types: {list(SUPPORTED_IMAGE_TYPES.keys()) + [SUPPORTED_PDF_TYPE]}"
            )
    except HTTPException:
        raise
    except Exception as e:
        raise HTTPException(status_code=400, detail=f"Error detecting file type: {str(e)}")

def process_image_ocr(image: Image.Image) -> str:
    """Extract text from an image using Tesseract OCR."""
    try:
        # Convert image to RGB if it's not already
        if image.mode != 'RGB':
            image = image.convert('RGB')
        
        # Extract text using Tesseract
        text = pytesseract.image_to_string(image, lang='eng')
        return text.strip()
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"OCR processing failed: {str(e)}")

def process_pdf_ocr(pdf_content: bytes) -> List[str]:
    """Extract text from PDF pages using OCR."""
    try:
        # Convert PDF to images
        images = convert_from_bytes(pdf_content)
        
        texts = []
        for i, image in enumerate(images):
            try:
                text = process_image_ocr(image)
                texts.append(text)
            except Exception as e:
                texts.append(f"Error processing page {i+1}: {str(e)}")
        
        return texts
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"PDF processing failed: {str(e)}")

def create_structured_response(texts: List[str], file_type: str, filename: str) -> Dict[str, Any]:
    """Create a structured response object."""
    if file_type == SUPPORTED_PDF_TYPE:
        return {
            "filename": filename,
            "file_type": file_type,
            "total_pages": len(texts),
            "pages": [
                {
                    "page_number": i + 1,
                    "extracted_text": text,
                    "character_count": len(text)
                }
                for i, text in enumerate(texts)
            ],
            "full_text": "\n\n".join(texts),
            "total_characters": sum(len(text) for text in texts)
        }
    else:
        # Single image
        text = texts[0] if texts else ""
        return {
            "filename": filename,
            "file_type": file_type,
            "extracted_text": text,
            "character_count": len(text)
        }

@app.get("/")
async def root():
    """Root endpoint with API information."""
    return {
        "message": "OCR PDF and Images API",
        "version": "1.0.0",
        "endpoints": {
            "upload": "/upload-file/",
            "health": "/health/"
        }
    }

@app.get("/health/")
async def health_check():
    """Health check endpoint."""
    try:
        # Test if Tesseract is available
        pytesseract.get_tesseract_version()
        return {"status": "healthy", "ocr_engine": "tesseract"}
    except Exception as e:
        return {"status": "unhealthy", "error": str(e)}

@app.post("/upload-file/")
async def upload_file_ocr(file: UploadFile = File(...)):
    """
    Upload a PDF or image file and extract text using OCR.
    
    Returns:
    - extracted_text: The text extracted from the file
    - structured_object: Organized data with metadata
    """
    try:
        # Read file content
        file_content = await file.read()
        
        if len(file_content) == 0:
            raise HTTPException(status_code=400, detail="Empty file uploaded")
        
        # Validate file type
        file_type = validate_file_type(file_content, file.filename or "unknown")
        
        # Process based on file type
        if file_type == SUPPORTED_PDF_TYPE:
            texts = process_pdf_ocr(file_content)
        else:
            # Process as image
            try:
                image = Image.open(io.BytesIO(file_content))
                text = process_image_ocr(image)
                texts = [text]
            except Exception as e:
                raise HTTPException(status_code=500, detail=f"Failed to process image: {str(e)}")
        
        # Create structured response
        structured_response = create_structured_response(texts, file_type, file.filename or "unknown")
        
        return {
            "success": True,
            "message": "File processed successfully",
            "data": structured_response
        }
        
    except HTTPException:
        raise
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Unexpected error: {str(e)}")

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)