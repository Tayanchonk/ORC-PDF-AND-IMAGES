import pytest
from fastapi.testclient import TestClient
from main import app
import io
from PIL import Image

client = TestClient(app)

def test_root_endpoint():
    """Test the root endpoint."""
    response = client.get("/")
    assert response.status_code == 200
    data = response.json()
    assert "message" in data
    assert "version" in data
    assert "endpoints" in data

def test_health_endpoint():
    """Test the health check endpoint."""
    response = client.get("/health/")
    assert response.status_code == 200
    data = response.json()
    assert "status" in data

def test_upload_empty_file():
    """Test uploading an empty file."""
    files = {"file": ("test.txt", b"", "text/plain")}
    response = client.post("/upload-file/", files=files)
    assert response.status_code == 400
    assert "Empty file uploaded" in response.json()["detail"]

def test_upload_unsupported_file():
    """Test uploading an unsupported file type."""
    files = {"file": ("test.txt", b"Hello World", "text/plain")}
    response = client.post("/upload-file/", files=files)
    assert response.status_code == 400
    assert "Unsupported file type" in response.json()["detail"]

def create_test_image():
    """Create a simple test image with text."""
    # Create a simple white image with black text
    img = Image.new('RGB', (200, 100), color='white')
    # Note: For a real test, we'd need to draw text on the image
    # For now, we'll just create a blank image
    
    img_bytes = io.BytesIO()
    img.save(img_bytes, format='PNG')
    img_bytes.seek(0)
    return img_bytes.getvalue()

def test_upload_image_file():
    """Test uploading a PNG image file."""
    test_image = create_test_image()
    files = {"file": ("test_image.png", test_image, "image/png")}
    response = client.post("/upload-file/", files=files)
    
    # The test should succeed even if OCR returns empty text for blank image
    assert response.status_code == 200
    data = response.json()
    assert data["success"] is True
    assert "data" in data
    assert "extracted_text" in data["data"]
    assert "filename" in data["data"]
    assert data["data"]["filename"] == "test_image.png"