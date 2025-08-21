using OcrApi.Models;

namespace OcrApi.Services
{
    public interface IOcrService
    {
        Task<OcrResponse> ProcessImageAsync(IFormFile file);
        Task<string> ExtractTextFromImageAsync(byte[] imageData);
        Task<HealthResponse> GetHealthStatusAsync();
        bool IsValidImageType(string contentType);
        bool IsValidPdfType(string contentType);
    }
}